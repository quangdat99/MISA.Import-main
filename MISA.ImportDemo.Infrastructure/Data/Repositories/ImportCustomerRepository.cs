using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces.Base;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Properties;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository phục vụ nhập khẩu khách hàng
    /// </summary>
    /// CreatedBy:  DQDAT (6/6/2021)
    public class ImportCustomerRepository : BaseImportRepository, IImportCustomerRepository
    {
        public DbConnection DbConnection { get => new MySqlConnection(_connectionString); }
        string _connectionString;
        IConfiguration _configuration;
        DynamicParameters Parameters;
        public ImportCustomerRepository(IEntityRepository entityRepository, IMemoryCache importMemoryCache, IConfiguration configuration) : base(entityRepository, importMemoryCache)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            Parameters = new DynamicParameters();
        }

        /// <summary>
        /// Thực hiện nhập khẩu khách hàng
        /// </summary>
        /// <param name="keyImport">Key xác định lấy dữ liệu để nhập khẩu từ cache</param>
        /// <param name="overriderData">Có cho phép ghi đè hay không (true- ghi đè dữ liệu trùng lặp trong db)</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết nhập khẩu)</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        public override async Task<ActionServiceResult> Import(string importKey, bool overriderData, CancellationToken cancellationToken)
        {
            var customers = ((List<Customer>)CacheGet(importKey)).Where(e => e.ImportValidState == ImportValidState.Valid || (overriderData && e.ImportValidState == ImportValidState.DuplicateInDb)).ToList(); ;

            using var dbContext = new EfDbContext();


            // Danh sách nhân viên thêm mới:
            var newCustomers = customers.Where(e => e.ImportValidState == Core.Enumeration.ImportValidState.Valid).ToList();
            await dbContext.Customer.AddRangeAsync(newCustomers);

            // Danh sách nhân viên thực hiện ghi đè:
            var modifiedCustomers = customers.Where(e => e.ImportValidState == Core.Enumeration.ImportValidState.DuplicateInDb).ToList();
            foreach (var customer in modifiedCustomers)
            {
                dbContext.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                var pbd = dbContext.CustomerReffered.Where(c => c.CustomerId == customer.CustomerId);
                dbContext.CustomerReffered.AddRange(customer.CustomerReffered);
                dbContext.CustomerReffered.RemoveRange(pbd);
            }
            //dbContext.Employee.UpdateRange(modifiedEmployees);
            await dbContext.SaveChangesAsync();
            return new ActionServiceResult(true, Resources.Msg_ImportSuccess, MISACode.Success, customers);

            //// Danh sách khách hàng hợp lệ
            //var customers = ((List<Customer>)CacheGet(importKey)).Where(e => e.ImportValidState == ImportValidState.Valid).ToList(); ;
            //var numberCustomerAddSuccess = 0;
            //var numberCustomerRefferedAddSuccess = 0;

            //// Thêm mới các khách hàng hợp lệ vào DB
            //customers.ForEach(cus => numberCustomerAddSuccess += InsertCustomer(cus));

            //// Thêm mới khách hàng được giới thiệu vào DB
            //customers.ForEach(cus => cus.CustomerReffered.ToList().ForEach(cusRef => numberCustomerRefferedAddSuccess += InsertCustomerReffered(cusRef)));


            //return new ActionServiceResult(true, String.Format(CustomerResource.Success_ImportCustomerAndCustomerReffered, numberCustomerAddSuccess, numberCustomerRefferedAddSuccess), MISACode.Success, customers);
        }


        /// <summary>
        /// Lấy toàn bộ danh sách khách hàng theo công ty
        /// </summary>
        /// <returns>Danh sách khách hàng đang có trong công ty</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        public async Task<List<Customer>> GetCustomers()
        {
            //var customers = await DbConnection.QueryAsync<Customer>("Proc_GetCustomers", commandType: CommandType.StoredProcedure);
            //return customers.ToList();

            using var dbContext = new EfDbContext();
            var customers = await dbContext.Customer.ToListAsync();
            return customers;
        }

        /// <summary>
        /// Hàm insert một khách hàng vào db.
        /// </summary>
        /// <param name="customer">Thông tin khách hàng.</param>
        /// <returns>Số khách hàng thêm thành công.</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        public int InsertCustomer(Customer customer)
        {
            Parameters.Add("CustomerId", customer.CustomerId);
            Parameters.Add("CustomerCode", customer.CustomerCode);
            Parameters.Add("FullName", customer.FullName);
            Parameters.Add("Gender", customer.Gender);
            Parameters.Add("DateOfBirth", customer.DateOfBirth);
            Parameters.Add("PhoneNumber", customer.PhoneNumber);
            Parameters.Add("CustomerGroupId", customer.CustomerGroupId);
            Parameters.Add("Email", customer.Email);
            Parameters.Add("NationalityId", customer.NationalityId);
            Parameters.Add("EthnicId", customer.EthnicId);
            Parameters.Add("CreatedBy", customer.CreatedBy);
            Parameters.Add("ModifiedBy", customer.ModifiedBy);
            Parameters.Add("CreatedDate", customer.CreatedDate);
            Parameters.Add("ModifiedDate", customer.ModifiedDate);

            var rowsAffect = DbConnection.Execute("Proc_InsertCustomer", Parameters, commandType: CommandType.StoredProcedure);
            return rowsAffect;
        }

        /// <summary>
        /// Hàm insert một khách hàng vào db.
        /// </summary>
        /// <param name="customer">Thông tin khách hàng.</param>
        /// <returns>Số khách hàng thêm thành công.</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        public int InsertCustomerReffered(CustomerReffered customerReffered)
        {
            Parameters.Add("CustomerRefferedId", customerReffered.CustomerRefferedId);
            Parameters.Add("CustomerCode", customerReffered.CustomerCode);
            Parameters.Add("FullName", customerReffered.FullName);
            Parameters.Add("PhoneNumber", customerReffered.PhoneNumber);
            Parameters.Add("Gender", customerReffered.Gender);

            var rowsAffect = DbConnection.Execute("Proc_CustomerReffered", Parameters, commandType: CommandType.StoredProcedure);
            return rowsAffect;
        }

    }
}
