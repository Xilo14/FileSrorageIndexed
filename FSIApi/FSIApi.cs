using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using UrlCombineLib;

namespace FSIApi {
    public class FSIApi {
        public Uri ApiUri { get; set; }

        public FSIApi(string uri) {
            ApiUri = new Uri(uri);
        }

        public async Task<IEnumerable<FileEntry>> SearchFiles(
            string query,
            bool isCheckName = true,
            bool isCheckDescription = true,
            bool isCheckContent = true,
            bool isCheckExt = true
        ) {
            var methodUri = ApiUri.Combine("SearchFiles");
            using var webClient = new WebClient();
            webClient.QueryString.Add("query", query);
            webClient.QueryString.Add("isCheckName", isCheckName ? "true" : "false");
            webClient.QueryString.Add("isCheckDescription", isCheckDescription ? "true" : "false");
            webClient.QueryString.Add("isCheckContent", isCheckContent ? "true" : "false");
            webClient.QueryString.Add("isCheckExt", isCheckExt ? "true" : "false");

            var response = await webClient.DownloadStringTaskAsync(methodUri);
            var result = JsonConvert.DeserializeObject<List<FileEntry>>(response);
            return result;
        }
        public async Task UploadFile(string name, string description, string path) {
            var methodUri = ApiUri.Combine("UploadFile").Combine($"?name={name}&description={description}");
            // var parameters = new System.Collections.Specialized.NameValueCollection()
            // {
            //     { "name", name },
            //     { "description", description }
            // };

            var request = new HttpRequestMessage(HttpMethod.Post, methodUri);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            var fileContent = new StreamContent(fs);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MimeTypes.GetMimeType(fs.Name));
            var form = new MultipartFormDataContent {
                { fileContent, "uploadedFile", fs.Name }
            };



            request.Content = form;

            var httpClient = new HttpClient();

            var response = await httpClient.SendAsync(request);
            // using var webClient = new WebClient();
            // webClient.QueryString = parameters;
            // var responseBytes = await webClient.UploadFileTaskAsync(methodUri, path);
            // var response = Encoding.UTF8.GetString(responseBytes);


        }
        public async Task GetFile(int id, string path) {
            var methodUri = ApiUri.Combine("GetFileById");
            var parameters = new System.Collections.Specialized.NameValueCollection()
            {
                { "id", id.ToString() }
            };

            using var webClient = new WebClient();
            webClient.QueryString = parameters;
            await webClient.DownloadFileTaskAsync(methodUri, path);
        }
        public async Task<int> GetFilesCount() {
            var methodUri = ApiUri.Combine("GetFilesCount");

            using var webClient = new WebClient();

            var response = await webClient.DownloadStringTaskAsync(methodUri);
            return Convert.ToInt32(response);
        }
    }
}
