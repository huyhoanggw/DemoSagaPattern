using DemoSaga.OrderService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DemoSaga.OrderApi.Database
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<OrderOutBoxMessage>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Topic).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Payload).IsRequired();
                entity.HasIndex(x => new { x.CreatedAtUtc, x.PublishedAtUtc });
            });
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderOutBoxMessage> OrderOutBoxMessages { get; set; }
    }
}
