namespace DemoSaga.ProductService.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal BasePrice { get; set; }
    }
}
