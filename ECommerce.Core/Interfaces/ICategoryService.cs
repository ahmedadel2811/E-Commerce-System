using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.category;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<Response<CategoryDetailDto>> GetCategoryByIdAsync(int id);
        Task<Response<IEnumerable<CategoryWithSubCategoriesDto>>> GetAllCategoriesAsync();
        Task<Response<IEnumerable<CategoryDto>>> GetMainCategoriesAsync();
        Task<Response<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(int parentCategoryId);
        Task<Response<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task<Response<CategoryDto>> UpdateCategoryAsync( UpdateCategoryDto categoryDto);
        Task<Response<bool>> DeleteCategoryAsync(int id);
        Task<Response<bool>> CategoryHasProductsAsync(int categoryId);
    }
}
