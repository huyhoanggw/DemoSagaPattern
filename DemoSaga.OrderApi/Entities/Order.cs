using Microsoft.VisualBasic;

namespace DemoSaga.OrderService.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
