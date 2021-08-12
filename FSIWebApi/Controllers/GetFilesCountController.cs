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
    public class GetFilesCountController : ControllerBase {
        private readonly ILogger<GetFilesCountController> _logger;
        private readonly FileStorageIndexed _fsIndexed;


        public GetFilesCountController(ILogger<GetFilesCountController> logger, FileStorageIndexed fsIndexed) {
            _logger = logger;
            _fsIndexed = fsIndexed;
        }

        [HttpGet]
        public async Task<int> Get() {
            return await _fsIndexed.GetCountFiles();
        }
    }
}
