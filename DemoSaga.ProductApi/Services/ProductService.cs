using DemoSaga.ProductService.Database;
using DemoSaga.ProductService.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoSaga.ProductService.Services
{
    public class ProductService(ProductDbContext dbcontext) : IProductService
    {
        public async Task<Product?> GetProduct(Guid id)
        {
            return await dbcontext.Products.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetProducts()
        {
            return await dbcontext.Products.ToListAsync();
        }
    }
}
