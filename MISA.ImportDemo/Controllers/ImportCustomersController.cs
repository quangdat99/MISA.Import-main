using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Controllers
{
    /// <summary>
    /// Service thực hiện nhập khẩu nhân viên
    /// </summary>
    /// Author:  DQDAT (6/6/2021)
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportCustomersController : BaseEntityController<ImportFileTemplate>
    {
        readonly IImportCustomerService _importService;

        /// <summary>
        /// Khởi tạo service
        /// </summary>
        /// <param name="importService"></param>
        public ImportCustomersController(IImportCustomerService importService) : base(importService)
        {
            _importService = importService;
        }

        /// <summary>
        /// Api thực hiện việc đọc và phân loại dữ liệu từ file Excel - hồ sơ lao động
        /// </summary>
        /// <param name="fileImport">Tệp nhập khẩu</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>200: Nhập khẩu thành công</returns>
        /// CreatedBy: DQDAT (6/6/2021)
        [HttpPost("reader")]
        public async Task<IActionResult> UploadImportFile(IFormFile fileImport, CancellationToken cancellationToken)
        {
            var res = await _importService.ReadCustomerDataFromExcel(fileImport, cancellationToken);
            return Ok(res);
        }

        /// <summary>
        /// Thực hiện nhập khẩu dữ liệu
        /// </summary>
        /// <param name="keyImport">Key xác định lấy dữ liệu để nhập khẩu từ cache</param>
        /// <param name="overriderData">Có cho phép ghi đè hay không (true- ghi đè dữ liệu trùng lặp trong db)</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>ActionResult(với các thông tin tương ứng tùy thuộc kết nhập khẩu)</returns>
        /// CreatedBy: DQDAT (6/6/2021)
        [HttpPost("{keyImport}")]
        public async Task<ActionResult<ImportFileTemplate>> Post(string keyImport, bool overriderData, CancellationToken cancellationToken)
        {
            return Ok(await _importService.Import(keyImport, overriderData, cancellationToken));
        }
    }
}
