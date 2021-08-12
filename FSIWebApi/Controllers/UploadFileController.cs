using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FSIBackend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FSIWebApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class UploadFileController : ControllerBase {
        private readonly ILogger<UploadFileController> _logger;
        private readonly FileStorageIndexed _fsIndexed;

        public UploadFileController(ILogger<UploadFileController> logger, FileStorageIndexed fsIndexed) {
            _logger = logger;
            _fsIndexed = fsIndexed;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile,
                                                 string name,
                                                 string description) {
var x = Request;
            if (uploadedFile is null) {
                return BadRequest();
            }

            var tmpStream = new MemoryStream();
            await uploadedFile.CopyToAsync(tmpStream);

            await _fsIndexed.AddFile(tmpStream, name, description, uploadedFile.ContentType);


            return Ok();
        }
    }
}
