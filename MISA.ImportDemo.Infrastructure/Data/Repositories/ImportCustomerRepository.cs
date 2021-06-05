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

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository phục vụ nhập khẩu nhân viên
    /// </summary>
    /// CreatedBy: NVMANH (12/12/2020)
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
        /// Lấy toàn bộ danh sách nhân viên theo công ty
        /// </summary>
        /// <returns>Danh sách nhân viên đang có trong công ty</returns>
        /// CreatedBy: NVMANH (12/12/2020)
        public async Task<List<Customer>> GetCustomers()
        {
            //var currentOrganizationId = CommonUtility.GetCurrentOrganizationId();
            //using var dbContext = new EfDbContext();
            //var customers = await dbContext.Customer.Where(e => e.OrganizationId == currentOrganizationId).ToListAsync();
            //return customers;

            var customers = await DbConnection.QueryAsync<Customer>("Proc_GetCustomers", commandType: CommandType.StoredProcedure);
            return customers.ToList();
        }
    }
}
