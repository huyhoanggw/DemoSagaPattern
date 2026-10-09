using DemoSaga.OrderService.Entities;

namespace DemoSaga.OrderService.Services
{
    public interface IOrderService
    {
        public Task<Order> AddOrder(Order order);
        public Task<List<Order>> GetOrdes();
    }
}
