namespace DemoSaga.OrderService.Model
{
    public class OrderModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
