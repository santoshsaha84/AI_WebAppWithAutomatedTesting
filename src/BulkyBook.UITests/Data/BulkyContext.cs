using Microsoft.EntityFrameworkCore;
using TEST_DATA_SETUP.Models;

namespace TEST_DATA_SETUP.Data
{
    public partial class BulkyContext : DbContext
    {
        public BulkyContext()
        {
        }

        public BulkyContext(DbContextOptions<BulkyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name)
                      .HasMaxLength(30);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryId");

                entity.Property(e => e.ImageUrl)
                      .HasDefaultValueSql("(N'')");

                entity.Property(e => e.Isbn)
                      .HasColumnName("ISBN");

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.CategoryId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
