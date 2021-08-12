using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

namespace FSIBackend {
    public class FileStorageDbContextFactory : IDesignTimeDbContextFactory<FileStorageDbContext> {
        public FileStorageDbContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<FileStorageDbContext>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();


            // получаем строку подключения из файла appsettings.json
            var connectionString = configuration["FSI:ConnectionString"];
            optionsBuilder.UseNpgsql(connectionString);

            return new FileStorageDbContext(optionsBuilder.Options);
        }
        public FileStorageDbContext CreateDbContext(string connectionString) {
            var optionsBuilder = new DbContextOptionsBuilder<FileStorageDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            return new FileStorageDbContext(optionsBuilder.Options);
        }
    }
}
