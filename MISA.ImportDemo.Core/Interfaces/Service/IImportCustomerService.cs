using Microsoft.AspNetCore.Http;
using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces
{
    /// <summary>
    /// Khai báo các hàm sử dụng cần thiết riêng cho service nhập khẩu khách hàng
    /// </summary>
    /// CreatedBy:  DQDAT (6/6/2021)
    public interface IImportCustomerService : IImportService
    {

        /// <summary>
        /// Thực hiện đọc tệp nhập khẩu
        /// </summary>
        /// <param name="formFile">Tệp nhập khẩu</param>
        /// <param name="cancellationToken">tham số custome phục vụ hủy Token khi cần thiết</param>
        /// <returns>Ok: nếu đọc thành công; 400: nếu có lỗi validate</returns>
        /// CreatedBy:  DQDAT (6/6/2021)
        Task<ActionServiceResult> ReadCustomerDataFromExcel(IFormFile formFile, CancellationToken cancellationToken);
    }
}
