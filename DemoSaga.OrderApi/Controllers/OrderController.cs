using DemoSaga.OrderService.Entities;
using DemoSaga.OrderService.Model;
using DemoSaga.OrderService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoSaga.OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService services) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel request)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = request.Price,
                OrderDate = DateTime.UtcNow
            };
            var result = await services.AddOrder(order);
            return result is not null ? Ok(result) : BadRequest("Failed to add order");
        }
        [HttpGet]
        public async Task<IActionResult> GetOrdes()
        {
            var result = await services.GetOrdes();
            return result is not null ? Ok(result) : BadRequest("Failed to retrieve orders");
        }
    }
}
