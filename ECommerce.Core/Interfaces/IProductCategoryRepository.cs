using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
    {

        Task AddCategoriesToProductAsync(int productId, List<int> categoryIds);
        Task RemoveCategoriesFromProductAsync(int productId, List<int> categoryIds);
        Task UpdateProductCategoriesAsync(int productId, List<int> categoryIds);
        Task<List<int>> GetProductCategoryIdsAsync(int productId);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Category>> GetProductCategoriesAsync(int productId);
    }
}
