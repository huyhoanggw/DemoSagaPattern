using DemoSaga.ProductService.Entities;

namespace DemoSaga.ProductService.Services
{
    public interface IProductService
    {
        public Task<List<Product>> GetProducts();
        public Task<Product?> GetProduct(Guid id);
    }
}
