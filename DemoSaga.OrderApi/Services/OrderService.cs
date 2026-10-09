using Confluent.Kafka;
using DemoSaga.OrderApi.Database;
using DemoSaga.OrderService.Entities;
using DemoSaga.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace DemoSaga.OrderService.Services
{


    public class OrderService(OrderDbContext dbcontext) : IOrderService
    {
        public async Task<Order> AddOrder(Order order)
        {
            dbcontext.Orders.Add(order);
            dbcontext.OrderOutBoxMessages.Add(new OrderOutBoxMessage
            {
                Id = Guid.NewGuid(),
                Topic = "order-created",
                Payload = JsonConvert.SerializeObject(order),
                CreatedAtUtc = DateTime.UtcNow,
            });
            await dbcontext.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetOrdes()
        {
            return await dbcontext.Orders.ToListAsync();
        }
    }
}
