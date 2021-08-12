using Microsoft.EntityFrameworkCore;
using FSIBackend.Models;

namespace FSIBackend {
    public class FileStorageDbContext : DbContext {
        public DbSet<File> Files { get; set; }
        public DbSet<Lemma> Lemmas { get; set; }
        public DbSet<LemmaEntry> LemmaEntries { get; set; }

        public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options)
            : base(options) {
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder
                .Entity<LemmaEntry>()
                .Property(e => e.FieldEntry)
                .HasConversion<int>();
        }
    }
}
