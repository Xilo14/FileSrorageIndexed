using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSIBackend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FSIWebApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class GetFileByIdController : ControllerBase {
        private readonly ILogger<GetFileByIdController> _logger;
        private readonly FileStorageIndexed _fsIndexed;


        public GetFileByIdController(ILogger<GetFileByIdController> logger, FileStorageIndexed fsIndexed) {
            _logger = logger;
            _fsIndexed = fsIndexed;
        }

        [HttpGet]
        public async Task<ActionResult> Get(int id) {
            FSIBackend.Models.File file = await _fsIndexed.GetFileById(id);
            return new FileStreamResult(await _fsIndexed.GetFileStream(file), file.ContentType ?? "") {
                FileDownloadName = file.OriginalName,
            };
        }
    }
}
