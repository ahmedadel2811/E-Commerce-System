using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.category;
using ECommerce.Core.DTOs.product;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IProductService
    {
        Task<Response<ProductDetailDto>> GetProductByIdAsync(int id);
        Task<Response<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<Response<IEnumerable<ProductDto>>> GetActiveProductsAsync();
        Task<Response<ProductDto>> CreateProductAsync(CreateProductDto productDto);
        Task<Response<ProductDto>> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<Response<bool>> DeleteProductAsync(int id);
        Task<Response<bool>> UpdateProductStockAsync(int productId, int quantity);
        Task<Response<IEnumerable<ProductDto>>> GetProductsByVendorAsync(int vendorId);


        Task<Response<bool>> AddCategoriesToProductAsync(int productId, List<int> categoryIds);
        Task<Response<bool>> UpdateProductCategoriesAsync(int productId, List<int> categoryIds);
        Task<Response<List<CategoryDto>>> GetProductCategoriesAsync(int productId);

        Task<Response<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId);


    }
}
