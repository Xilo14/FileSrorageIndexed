using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FSIBackend;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FSIBackend.Models;

namespace FSIWebApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class SearchFilesController : ControllerBase {
        private readonly ILogger<SearchFilesController> _logger;
        private readonly FileStorageIndexed _fsIndexed;

        public SearchFilesController(ILogger<SearchFilesController> logger, FileStorageIndexed fsIndexed) {
            _logger = logger;
            _fsIndexed = fsIndexed;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<FileEntry>> Search(
            string query,
            bool isCheckName,
            bool isCheckDescription,
            bool isCheckContent,
            bool isCheckExt) {
            if (!(isCheckName | isCheckDescription | isCheckContent | isCheckExt)) {
                return new List<FileEntry>();
            }

            var result = (await _fsIndexed.SearchFiles(
                query,
                isCheckName,
                isCheckDescription,
                isCheckContent,
                isCheckExt))
            .Select(e => new FileEntry() {
                Name = e.Key.OriginalName,
                Description = e.Key.Description,
                Rank = e.Value,
                Id = e.Key.FileId,
            }).ToList();
            result.Sort((a1, a2) => a1.Rank.CompareTo(a2.Rank));
            return result;

        }
        public class FileEntry {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Rank { get; set; }
            public int Id { get; set; }
        }
    }
}
