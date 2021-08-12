using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FSIApi;

namespace LemmaTest {
    class Program {
        static void Main(string[] args) {
            var api = new FSIApi.FSIApi("https://localhost:5001");

            Console.WriteLine(api.GetFilesCount().Result);

            var path = "test.png";

            //api.UploadFile("test.png", "Тестовый файл", path).Wait();


            var result = api.SearchFiles("Тестовый файл").Result.ToList();

            var outPath = "out.png";

            api.GetFile(result[0].Id, outPath).Wait();

            Console.WriteLine("Complete!");
            Console.ReadLine();

        }
    }
}
