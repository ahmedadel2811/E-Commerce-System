using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
        Task<Product> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetProductsByVendorAsync(int vendorId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task UpdateProductStockAsync(int productId, int quantity);
    }
}
