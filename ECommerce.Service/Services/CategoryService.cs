using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Core.DTOs.category;
using ECommerce.Core.DTOs.product;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Response<CategoryDetailDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
                if (category == null)
                    return Response<CategoryDetailDto>.Fail("Category not found");

                var categoryDto = MapToCategoryDetailDto(category);
                return Response<CategoryDetailDto>.Success(categoryDto, "Category retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<CategoryDetailDto>.Fail($"Error retrieving category: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<CategoryWithSubCategoriesDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesWithSubCategoriesAsync();
                var categoryDtos = categories.Select(MapToCategoryWithSubCategoriesDto).ToList();
                return Response<IEnumerable<CategoryWithSubCategoriesDto>>.Success(categoryDtos, "Categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<CategoryWithSubCategoriesDto>>.Fail($"Error retrieving categories: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<CategoryDto>>> GetMainCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetMainCategoriesAsync();
                var categoryDtos = categories.Select(MapToCategoryDto).ToList();
                return Response<IEnumerable<CategoryDto>>.Success(categoryDtos, "Main categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<CategoryDto>>.Fail($"Error retrieving main categories: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(int parentCategoryId)
        {
            try
            {
                var categories = await _categoryRepository.GetSubCategoriesAsync(parentCategoryId);

                if (categories?.Any() != true)
                {
                    return Response<IEnumerable<CategoryDto>>.Fail("categories not found");
                }
                var categoryDtos = categories.Select(MapToCategoryDto).ToList();
                return Response<IEnumerable<CategoryDto>>.Success(categoryDtos, "Subcategories retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<CategoryDto>>.Fail($"Error retrieving subcategories: {ex.Message}");
            }
        }

        public async Task<Response<CategoryDto>> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            try
            {
                // Check if parent category exists
                if (categoryDto.ParentCategoryId.HasValue)
                {
                    var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.ParentCategoryId.Value);
                    if (parentCategory == null)
                        return Response<CategoryDto>.Fail("Parent category not found");
                }

                var category = new Category
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    ImageUrl = categoryDto.ImageUrl,
                    ParentCategoryId = categoryDto.ParentCategoryId,
                    CreatedAt = DateTime.UtcNow
                };

                var createdCategory = await _categoryRepository.AddAsync(category);
                var resultDto = MapToCategoryDto(createdCategory);
                return Response<CategoryDto>.Success(resultDto, "Category created successfully");
            }
            catch (Exception ex)
            {
                return Response<CategoryDto>.Fail($"Error creating category: {ex.Message}");
            }
        }

        public async Task<Response<CategoryDto>> UpdateCategoryAsync( UpdateCategoryDto categoryDto)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
                if (existingCategory == null)
                    return Response<CategoryDto>.Fail("Category not found");

                // Check if parent category exists
                // true 9     9== 9 
                if (categoryDto.ParentCategoryId.HasValue && categoryDto.ParentCategoryId.Value == categoryDto.Id)
                {
                    var parentCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
                    if (parentCategory.Id==categoryDto.ParentCategoryId)
                        return Response<CategoryDto>.Fail("Parent category not found");
                }

                // Update properties
                existingCategory.Name = categoryDto.Name;
                existingCategory.Description = categoryDto.Description;
                existingCategory.ImageUrl = categoryDto.ImageUrl;
                existingCategory.ParentCategoryId = categoryDto.ParentCategoryId;
                existingCategory.UpdatedAt = DateTime.UtcNow;

                await _categoryRepository.UpdateAsync(existingCategory);
                var updatedDto = MapToCategoryDto(existingCategory);
                return Response<CategoryDto>.Success(updatedDto, "Category updated successfully");
            }
            catch (Exception ex)
            {
                return Response<CategoryDto>.Fail($"Error updating category: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return Response<bool>.Fail("Category not found");

                // Check if category has products
                if (await _categoryRepository.CategoryHasProductsAsync(id))
                    return Response<bool>.Fail("Cannot delete category that has products");

                // Check if category has subcategories
                var subCategories = await _categoryRepository.GetSubCategoriesAsync(id);
                if (subCategories.Any())
                    return Response<bool>.Fail("Cannot delete category that has subcategories");

                await _categoryRepository.DeleteAsync(category);
                return Response<bool>.Success(true, "Category deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error deleting category: {ex.Message}");
            }
        }

        public async Task<Response<bool>> CategoryHasProductsAsync(int categoryId)
        {

            try
            {
                var hasProducts = await _categoryRepository.CategoryHasProductsAsync(categoryId);

                if (hasProducts)
                {
                    //  فيها منتجات
                    return Response<bool>.Success(true, "Category contains products");
                }
                else
                {
                    //  مفيهاش منتجات
                    return Response<bool>.Success(false, "Category has no products");
                }
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error checking category products: {ex.Message}");
            }
        }

        // Manual Mapping Methods
        private CategoryDto MapToCategoryDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                ProductsCount = category.Products?.Count ?? 0,
            };
        }

        private CategoryDetailDto MapToCategoryDetailDto(Category category)
        {
            return new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
                SubCategories = category.SubCategories?.Select(MapToCategoryDto).ToList() ?? new List<CategoryDto>(),

                Products = category.Products?.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    FinalPrice = p.FinalPrice,
                    SKU=p.SKU,
                    StockQuantity=p.StockQuantity,
                    CategoryName=p.Category.Name,
                    VendorShopName=p.Vendor?.ShopName,
                    DiscountPercent=p.DiscountPercent,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive
                }).ToList() ?? new List<ProductDto>()
            };
        }

        private CategoryWithSubCategoriesDto MapToCategoryWithSubCategoriesDto(Category category)
        {
            return new CategoryWithSubCategoriesDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                SubCategories = category.SubCategories?.Select(MapToCategoryDto).ToList() ?? new List<CategoryDto>()
            };
        }
    }
}