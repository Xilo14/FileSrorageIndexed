using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FSIBackend.Models;
using System.Collections.Generic;

namespace FSIBackend {
    public class FileStorageIndexed {
        public readonly string FolderPath;
        private readonly string _connectionString;

        public FileStorageIndexed(IConfigurationSection section) {
            if (Path.IsPathFullyQualified(section["FolderPath"]))
                FolderPath = section["FolderPath"];
            else
                FolderPath = Path.Join(Environment.CurrentDirectory, section["FolderPath"]);

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);


            _connectionString = section["ConnectionString"];
        }

        public async Task<Stream> GetFileStream(Models.File file) {
            return new FileStream(FolderPath + "/" + file.Path, FileMode.Open, FileAccess.Read);
        }

        public async Task<Models.File> GetFileById(int id) {
            var db = new FileStorageDbContextFactory().CreateDbContext(_connectionString);
            return await db.Files.FirstOrDefaultAsync(e => e.FileId == id);
        }

        public async Task AddFile(Stream stream,
                                  string name,
                                  string description,
                                  string contentType) {
            var db = new FileStorageDbContextFactory().CreateDbContext(_connectionString);

            var guid = Guid.NewGuid();

            var file = new Models.File() {
                OriginalName = name,
                Description = description,
                Path = guid.ToString(),
                ContentType = contentType,
            };
            await db.Files.AddAsync(file);
            var fileStream = new FileStream(FolderPath + "/" + file.Path, FileMode.Create);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);
            await fileStream.DisposeAsync();


            await AddFileToIndex(db, file);
            await db.SaveChangesAsync();
        }

        public async Task<IDictionary<Models.File, int>> SearchFiles(string query,
                                                                bool isCheckName,
                                                                bool isCheckDescription,
                                                                bool isCheckContent,
                                                                bool isCheckExt) {
            var db = new FileStorageDbContextFactory().CreateDbContext(_connectionString);
            var queryLemmas = await Lemmalizer.Lemmalizer.LemmalizeToDict(query);
            var foundedLemmas = db.Lemmas.ToList().Where(e => queryLemmas.ContainsKey(e.Value));
            var foundedLemmasIds = foundedLemmas.Select(e => e.LemmaId);


            var lemmaEntries = db.LemmaEntries
                .Include(e => e.File)
                .Where(e => foundedLemmasIds.Contains(e.LemmaId));

            if (!isCheckName)
                lemmaEntries = lemmaEntries.Where(e => e.FieldEntry != LemmaEntry.FieldEntriesEnum.OriginalName);
            if (!isCheckDescription)
                lemmaEntries = lemmaEntries.Where(e => e.FieldEntry != LemmaEntry.FieldEntriesEnum.Description);
            if (!isCheckContent)
                lemmaEntries = lemmaEntries.Where(e => e.FieldEntry != LemmaEntry.FieldEntriesEnum.Content);
            if (!isCheckExt)
                lemmaEntries = lemmaEntries.Where(e => e.FieldEntry != LemmaEntry.FieldEntriesEnum.ContentType);

            var foundedFiles = new Dictionary<Models.File, int>();
            foreach (var lemmaEntry in lemmaEntries) {
                if (foundedFiles.ContainsKey(lemmaEntry.File))
                    foundedFiles[lemmaEntry.File] += lemmaEntry.Count;
                else
                    foundedFiles.Add(lemmaEntry.File, lemmaEntry.Count);
            }

            return foundedFiles;
        }
        private async Task AddFileToIndex(FileStorageDbContext context, Models.File file) {
            var nameLemmas = await Lemmalizer.Lemmalizer.LemmalizeToDict(file.OriginalName);
            var descriptionLemmas = await Lemmalizer.Lemmalizer.LemmalizeToDict(file.Description);
            var contentLemmas = await Lemmalizer.Lemmalizer.LemmalizeToDict(
                await TryGetContent(FolderPath + "/" + file.Path));
            var contentTypeLemmas = await Lemmalizer.Lemmalizer.LemmalizeToDict(file.ContentType);

            foreach (var lemma in nameLemmas) {
                var lemmaEntity = await context.Lemmas.FirstOrDefaultAsync(e => e.Value == lemma.Key);
                if (lemmaEntity == null) {
                    lemmaEntity = new Models.Lemma() { Value = lemma.Key };
                    await context.Lemmas.AddAsync(lemmaEntity);
                }
                await context.LemmaEntries.AddAsync(new LemmaEntry() {
                    File = file,
                    Lemma = lemmaEntity,
                    Count = (int)lemma.Value,
                    FieldEntry = LemmaEntry.FieldEntriesEnum.OriginalName
                });
            }
            foreach (var lemma in descriptionLemmas) {
                var lemmaEntity = await context.Lemmas.FirstOrDefaultAsync(e => e.Value == lemma.Key);
                if (lemmaEntity == null) {
                    lemmaEntity = new Models.Lemma() { Value = lemma.Key };
                    await context.Lemmas.AddAsync(lemmaEntity);
                }
                await context.LemmaEntries.AddAsync(new LemmaEntry() {
                    File = file,
                    Lemma = lemmaEntity,
                    Count = (int)lemma.Value,
                    FieldEntry = LemmaEntry.FieldEntriesEnum.Description
                });
            }
            foreach (var lemma in contentLemmas) {
                var lemmaEntity = await context.Lemmas.FirstOrDefaultAsync(e => e.Value == lemma.Key);
                if (lemmaEntity == null) {
                    lemmaEntity = new Models.Lemma() { Value = lemma.Key };
                    await context.Lemmas.AddAsync(lemmaEntity);
                }
                await context.LemmaEntries.AddAsync(new LemmaEntry() {
                    File = file,
                    Lemma = lemmaEntity,
                    Count = (int)lemma.Value,
                    FieldEntry = LemmaEntry.FieldEntriesEnum.Content
                });
            }
            foreach (var lemma in contentTypeLemmas) {
                var lemmaEntity = await context.Lemmas.FirstOrDefaultAsync(e => e.Value == lemma.Key);
                if (lemmaEntity == null) {
                    lemmaEntity = new Models.Lemma() { Value = lemma.Key };
                    await context.Lemmas.AddAsync(lemmaEntity);
                }
                await context.LemmaEntries.AddAsync(new LemmaEntry() {
                    File = file,
                    Lemma = lemmaEntity,
                    Count = (int)lemma.Value,
                    FieldEntry = LemmaEntry.FieldEntriesEnum.ContentType
                });
            }

        }

        public async Task<int> GetCountFiles() {
            var db = new FileStorageDbContextFactory().CreateDbContext(_connectionString);
            return await db.Files.CountAsync();
        }
        private async Task<string> TryGetContent(string Path) {
            using var reader = new StreamReader(Path, Encoding.Default);
            var bytesRead = await reader.ReadToEndAsync();
            reader.Close();

            for (var i = 0; i < bytesRead.Length; i++) {
                var c = bytesRead[i];


                if (char.IsControl(c)
                && c != (char)10  // New line
                && c != (char)13 // Carriage Return
                && c != (char)11 // Tab
                )
                    return "";
            }

            return bytesRead;
        }
    }
}
