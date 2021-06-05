using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Repository
{
    /// <summary>
    /// interface quản lý nghiệp vụ phần đơn vị
    /// </summary>
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomerByFilter(object[] filter);


    }
}
