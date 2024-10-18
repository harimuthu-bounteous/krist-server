using krist_server.Models;
using Microsoft.EntityFrameworkCore;

namespace krist_server.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.ProductId)
                .HasDefaultValueSql("gen_random_uuid()");  // Use UUID generator for PostgreSQL
        }
    }
}