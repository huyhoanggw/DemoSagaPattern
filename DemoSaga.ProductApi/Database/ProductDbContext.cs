using DemoSaga.ProductService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DemoSaga.ProductService.Database
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Quantity = 100,
                BasePrice = 1000
            });
            modelBuilder.Entity<Product>().HasData(new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                Quantity = 200,
                BasePrice = 200
            });
            modelBuilder.Entity<Product>().HasData(new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Product 3",
                Quantity = 300,
                BasePrice = 700
            });


            modelBuilder.Entity<Product>().HasKey(o => o.Id);
            modelBuilder.Entity<ProductOutBoxMessage>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Topic).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Payload).IsRequired();
                entity.HasIndex(x => new { x.CreatedAtUtc, x.PublishedAtUtc });
            });
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOutBoxMessage> ProductOutBoxMessages { get; set; }
    }
}
