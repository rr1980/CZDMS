using CZDMS.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace CZDMS.Db
{
    public class FileDbContext : DbContext
    {
        public FileDbContext() { }
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CZDMS;Trusted_Connection=True;ConnectRetryCount=0");
            }
        }

        public virtual DbSet<FileItem> FileItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<FileItem>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<FileItem>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Gcrecord);
                entity.Property(e => e.IsFolder).HasDefaultValueSql("((0))");
                entity.Property(e => e.LastWriteTime);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.ParentId);
                entity.Property(e => e.SsmaTimeStamp)
                    .IsRequired()
                    .HasColumnName("SSMA_TimeStamp")
                    .IsRowVersion();
            });
        }
    }
}
