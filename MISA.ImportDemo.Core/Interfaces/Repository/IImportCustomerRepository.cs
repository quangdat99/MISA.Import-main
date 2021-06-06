using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Repository
{
    /// <summary>
    /// Interface khai báo các hàm cung cấp cho việc nhập khẩu khách hàng
    /// </summary>
    /// CreatedBy:  DQDAT (6/6/2021)
    public interface IImportCustomerRepository:IBaseImportRepository
    {

        /// <summary>
        /// Lấy toàn bộ danh sách khách hàng có trong Db
        /// </summary>
        /// <returns>List Khách hàng</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        Task<List<Customer>> GetCustomers();
    }
}
