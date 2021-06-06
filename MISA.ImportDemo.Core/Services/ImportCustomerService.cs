using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Services
{
    /// <summary>
    /// Service xử lý việc nhập khẩu khách hàng
    /// </summary>
    /// CreatedBy:  DQDAT (6/6/2021)
    public class ImportCustomerService : BaseImportService, IImportCustomerService
    {
        public ImportCustomerService(IImportCustomerRepository importRepository, IMemoryCache importMemoryCache) : base(importRepository, importMemoryCache, "Customer")
        {

        }

        /// <summary>
        /// Thực hiện nhập khẩu dữ liệu
        /// </summary>
        /// <param name="keyImport">Key xác định lấy dữ liệu để nhập khẩu từ cache</param>
        /// <param name="overriderData">Có cho phép ghi đè hay không (true- ghi đè dữ liệu trùng lặp trong db)</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết nhập khẩu)</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        public async Task<ActionServiceResult> Import(string keyImport, bool overriderData, CancellationToken cancellationToken)
        {
            return await _importRepository.Import(keyImport, overriderData, cancellationToken);
        }

        /// <summary>
        /// Thực hiện đọc dữ liệu từ tệp nhập khẩu
        /// </summary>
        /// <param name="importFile">Tệp nhập khẩu</param>
        /// <param name="cancellationToken">Tham số tùy chọn sử dụng xử lý Task đa luồng</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết quả đọc tệp)</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        public async Task<ActionServiceResult> ReadCustomerDataFromExcel(IFormFile importFile, CancellationToken cancellationToken)
        {
            // Lấy dữ liệu nhân viên trên Db về để thực hiện check trùng:
            EntitiesFromDatabase = (await GetCustomersFromDatabase()).Cast<object>().ToList();
            var customers = await base.ReadDataFromExcel<Customer>(importFile, cancellationToken);
            var importInfo = new ImportInfo(String.Format("CustomerImport_{0}", Guid.NewGuid()), customers);
            // Lưu dữ liệu vào cache:
            importMemoryCache.Set(importInfo.ImportKey, customers);
            return new ActionServiceResult(true, Resources.Msg_ImportFileReadSuccess, MISACode.Success, importInfo);
        }

        /// <summary>
        ///  Lấy toàn bộ danh sách khách hàng đang có trong Database theo từng công ty.
        ///  với bộ hồ sơ (ProfileBook) đang nhập khẩu vào - lưu vào cache để thực hiện check trùng
        /// </summary>
        /// CreatedBy:  DQDAT (6/6/2021)
        private async Task<List<Customer>> GetCustomersFromDatabase()
        {
            var importRepository = _importRepository as IImportCustomerRepository;
            return await importRepository.GetCustomers();

        }

        /// <summary>
        /// Check trùng dữ liệu trong File Excel và trong database, dựa vào mã khách hàng
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="entitiesInFile">Danh sách các đối tượng được build từ tệp nhập khẩu</param>
        /// <param name="entity">thực thể hiện tại</param>
        /// <param name="cellValue">Giá trị nhập trong ô excel đang đọc</param>
        /// <param name="importColumn">Thông tin cột nhập khẩu (tiêu đề cột, kiểu giá trị....)</param>
        /// CreatedBy:  DQDAT (6/6/2021)
        protected override void CheckDuplicateData<T>(List<T> entitiesInFile, T entity, object cellValue, ImportColumn importColumn)
        {
            if (entity is Customer)
            {
                var newCustomer = entity as Customer;
                // Validate: kiểm tra trùng dữ liệu trong File Excel và trong Database: check theo Mã khách hàng
                if (importColumn.ColumnInsert == "CustomerCode" && cellValue != null)
                {
                    var customerCode = cellValue.ToString().Trim();
                    // Check trong File
                    var itemDuplicate = entitiesInFile.Where(item => (item.GetType().GetProperty("CustomerCode").GetValue(item) ?? string.Empty).ToString() == customerCode).FirstOrDefault();
                    if (itemDuplicate != null)
                    {
                        entity.ImportValidState = ImportValidState.DuplicateInFile;
                        entity.ImportValidError.Add(string.Format(CustomerResource.Error_ImportCustomerCodeDuplicateInFile, customerCode));
                    }
                    // Check trong Db:
                    var itemDuplicateInDb = EntitiesFromDatabase.Where(item => (item.GetType().GetProperty("CustomerCode").GetValue(item) ?? string.Empty).ToString() == customerCode).Cast<T>().FirstOrDefault();
                    if (itemDuplicateInDb != null)
                    {
                        entity.ImportValidState = ImportValidState.DuplicateInDb;
                        entity.ImportValidError.Add(string.Format(CustomerResource.Error_ImportCustomerCodeDuplicateInDatabase, customerCode));
                   }
                }
            }
            else
            {
                base.CheckDuplicateData(entitiesInFile, entity, cellValue, importColumn);
            }
        }


        /// <summary>
        /// Khởi tạo đối tượng trước khi build các thông tin
        /// Dựa vào thông tin bảng dữ liệu sẽ import dữ liệu vào mà map các đối tượng tương ứng.
        /// </summary>
        /// <typeparam name="T">Kiểu của object</typeparam>
        /// <returns>Thực thể được khởi tạo với kiểu tương ứng</returns>
        /// OverriderBy:  DQDAT (6/6/2021)
        protected override dynamic InstanceEntityBeforeMappingData<T>()
        {
            var ImportToTable = ImportWorksheetTemplate.ImportToTable;
            switch (ImportToTable)
            {
                case "Customer":
                    var newEntity = new Customer();
                    newEntity.CustomerId = Guid.NewGuid();
                    return newEntity;
                case "CustomerReffered":
                    var cReffered = new CustomerReffered()
                    {
                        CustomerRefferedId = Guid.NewGuid()
                    }; //Activator.CreateInstance("MISA.ImportDemo.Core.Entities", "ProfileFamilyDetail");
                    return cReffered;
                default:
                    return base.InstanceEntityBeforeMappingData<T>();
            }
        }

        /// <summary>
        /// Sau khi các thông tin được build hoàn chỉnh thì làm một số việc cần thiết
        /// 1. Mapping dữ liệu thông tin thành viên gia đình tương ứng với nhân viên nào
        /// 2. Validate có lỗi gì cụ thể
        /// </summary>
        /// <typeparam name="T">kiểu của object</typeparam>
        /// <param name="entity">object thành viên trong gia đình</param>
        /// OverriderBy:  DQDAT (6/6/2021)
        protected override void ProcessDataAfterBuild<T>(object entity)
        {
            if (entity is CustomerReffered)
            {
                var customerReffered = entity as CustomerReffered;
                var customerCode = customerReffered.CustomerCode;
                var customerMaster = _entitiesFromEXCEL.Cast<Customer>().Where(pbd => pbd.CustomerCode == customerCode).FirstOrDefault();
                if (customerMaster != null && customerCode != null)
                {
                    customerMaster.CustomerReffered.Add(customerReffered);

                    // Duyệt từng lỗi của detail và add thông tin vào master:
                    foreach (var importValidError in customerReffered.ImportValidError)
                    {
                        customerMaster.ImportValidError.Add(String.Format("Thông tin khách hàng được giới thiệu: {0} - {1}", customerReffered.FullName, importValidError));
                    }

                    // Nếu master không có lỗi valid, detail có thì gán lại cờ cho master là invalid:
                    if (customerReffered.ImportValidState != ImportValidState.Valid && customerMaster.ImportValidState == ImportValidState.Valid)
                        customerMaster.ImportValidState = ImportValidState.Invalid;
                }
            }
            base.ProcessDataAfterBuild<T>(entity);
        }

        /// <summary>
        /// Xử lý dữ liệu liên quan đến ngày/ tháng
        /// </summary>
        /// <param name="entity">Thực thế sẽ import vào Db</param>
        /// <param name="cellValue">Giá trị của cell</param>
        /// <param name="type">Kiểu dữ liệu</param>
        /// <param name="importColumn">Thông tin cột import được khai báo trong Db</param>
        /// <returns>giá trị ngày tháng được chuyển đổi tương ứng</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        protected override DateTime? GetProcessDateTimeValue<T>(T entity, object cellValue, Type type, ImportColumn importColumn = null)
        {
            if ((entity is Customer && importColumn.ColumnInsert == "DateOfBirth"))
            {
                try
                {
                    return DateTime.ParseExact(cellValue.ToString(), new string[] { "dd/MM/yyyy", "MM/yyyy", "yyyy", "d/M/yyyy", "dd/yyyy", "dd/M/yyyy", "d/MM/yyyy", "M/yyyy", "d/yyyy" }, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    return base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);

                }
               
            }
            else if (entity is CustomerReffered && importColumn.ColumnInsert == "DateOfBirth")
            {
                try
                {
                    return DateTime.ParseExact(cellValue.ToString(), new string[] { "dd/MM/yyyy", "MM/yyyy", "yyyy", "d/M/yyyy", "dd/yyyy", "dd/M/yyyy", "d/MM/yyyy", "M/yyyy", "d/yyyy" }, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    return base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);

                }
            }
            else
                return base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);
        }

    }
}
