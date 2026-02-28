using ECommerce.Core.DTOs.category;
using ECommerce.Core.DTOs.product;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        private readonly IProductCategoryRepository _productCategoryRepository;

        //private readonly IVendorRepository _vendorRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IProductCategoryRepository productCategoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<Response<ProductDetailDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetProductWithDetailsAsync(id);
                if (product == null)
                    return Response<ProductDetailDto>.Fail("Product not found");

                var productDto = MapToProductDetailDto(product);
                return Response<ProductDetailDto>.Success(productDto, "Product retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductDetailDto>.Fail($"Error retrieving product: {ex.Message}");
            }
        }

        public async Task<Response<IQueryable<ProductDto>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetProductsWithCategoryAsync();
                var productDtos = products.Select(MapToProductDto);

                return Response<IQueryable<ProductDto>>.Success(productDtos, "Products retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ProductDto>>.Fail($"Error retrieving products: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ProductDto>>> GetActiveProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetActiveProductsAsync();
                var productDtos = products.Select(MapToProductDto).ToList();
                return Response<IEnumerable<ProductDto>>.Success(productDtos, "Active products retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ProductDto>>.Fail($"Error retrieving active products: {ex.Message}");
            }
        }

        public async Task<Response<ProductDto>> CreateProductAsync(CreateProductDto productDto)
        {
            try
            {
                //// Validate vendor exists
                //var vendor = await _vendorRepository.GetByIdAsync(productDto.VendorId);
                //if (vendor == null)
                //    return Response<ProductDto>.Fail("Vendor not found");

                // Validate category exists if provided
                if (productDto.CategoryId.HasValue)
                {
                    var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId.Value);
                    if (category == null)
                        return Response<ProductDto>.Fail("Category not found");
                }

                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    DiscountPercent = productDto.DiscountPercent,
                    SKU = productDto.SKU,
                    StockQuantity = productDto.StockQuantity,
                    VendorId = productDto?.VendorId,
                    CategoryId = productDto.CategoryId,
                    ImageUrl = productDto.ImageUrl,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdProduct = await _productRepository.AddAsync(product);
                var resultDto = MapToProductDto(createdProduct);
                return Response<ProductDto>.Success(resultDto, "Product created successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductDto>.Fail($"Error creating product: {ex.Message}");
            }
        }

        public async Task<Response<ProductDto>> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsyncAndInclude(id,
                    q => q.Include(x => x.Category)
                    );

                if (existingProduct == null)
                    return Response<ProductDto>.Fail("Product not found");

                // Update properties
                existingProduct.Name = productDto.Name;
                existingProduct.Description = productDto.Description;
                existingProduct.Price = productDto.Price;
                existingProduct.DiscountPercent = productDto.DiscountPercent;
                existingProduct.StockQuantity = productDto.StockQuantity;
                existingProduct.CategoryId = productDto.CategoryId;
                existingProduct.ImageUrl = productDto.ImageUrl;
                existingProduct.IsActive = productDto.IsActive;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _productRepository.UpdateAsync(existingProduct);
                var updatedDto = MapToProductDto(existingProduct);
                return Response<ProductDto>.Success(updatedDto, "Product updated successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductDto>.Fail($"Error updating product: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return Response<bool>.Fail("Product not found");

                await _productRepository.DeleteAsync(product);
                return Response<bool>.Success(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error deleting product: {ex.Message}");
            }
        }

        public async Task<Response<bool>> UpdateProductStockAsync(int productId, int quantity)
        {
            try
            {
                //    هشوف الاول المنتج موجودولا لا 
                var existingProduct = await _productRepository.GetByIdAsync(productId);
                if (existingProduct == null)
                {
                    return Response<bool>.Fail("Product not found");
                }

                await _productRepository.UpdateProductStockAsync(productId, quantity);
                return Response<bool>.Success(true, "Product stock updated successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error updating product stock: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ProductDto>>> GetProductsByVendorAsync(int vendorId)
        {
            try
            {
                var products = await _productRepository.GetProductsByVendorAsync(vendorId);

                if (products?.Any() != true)
                {
                    return Response<IEnumerable<ProductDto>>.Fail("Vendor  dont have any products  ");

                }
                var productDtos = products.Select(MapToProductDto).ToList();
                return Response<IEnumerable<ProductDto>>.Success(productDtos, "Vendor products retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ProductDto>>.Fail($"Error retrieving vendor products: {ex.Message}");
            }
        }

        // Manual Mapping Methods
        private ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                FinalPrice = product.FinalPrice,
                SKU = product.SKU,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryName = product.Category?.Name,
                VendorShopName = product.Vendor?.ShopName,
                DiscountPercent = product.DiscountPercent
            };
        }

        private ProductDetailDto MapToProductDetailDto(Product product)
        {
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                FinalPrice = product.FinalPrice,
                SKU = product.SKU,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name,
                VendorShopName = product.Vendor?.ShopName,
                //Images = product.Images?.Select(i => new ProductImageDto
                //{
                //    Id = i.Id,
                //    ImageUrl = i.ImageUrl,
                //    IsMain = i.IsMain
                //}).ToList() ?? new List<ProductImageDto>(),
                //Variants = product.Variants?.Select(v => new ProductVariantDto
                //{
                //    Id = v.Id,
                //    Color = v.Color,
                //    Size = v.Size,
                //    AdditionalPrice = v.AdditionalPrice,
                //    StockQuantity = v.StockQuantity
                //}).ToList() ?? new List<ProductVariantDto>(),
                //Reviews = product.Reviews?.Select(r => new ProductReviewDto
                //{
                //    Id = r.Id,
                //    Rating = r.Rating,
                //    Comment = r.Comment,
                //    UserName = r.User?.FullName,
                //    CreatedAt = r.CreatedAt
                //}).ToList() ?? new List<ProductReviewDto>()
            };
        }

        public async Task<Response<bool>> AddCategoriesToProductAsync(int productId, List<int> categoryIds)
        {
            try
            {
                //   تحقق من وجود المنتج
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return Response<bool>.Fail("Product not found");
                }

                //   تحقق من أن القائمة مش فاضية
                if (categoryIds == null || !categoryIds.Any())
                {
                    return Response<bool>.Fail("No categories provided");
                }

                //   تحقق من أن كل الكاتيجوريز موجودة فعلًا في قاعدة البيانات
                var existingCategoryIds = await _categoryRepository
                    .GetAllAsync()
                    .ContinueWith(t => t.Result.Select(c => c.Id).ToList());

                var invalidIds = categoryIds.Except(existingCategoryIds).ToList();
                if (invalidIds.Any())
                {
                    return Response<bool>.Fail($"Invalid category IDs: {string.Join(", ", invalidIds)}");
                }

                //   تحقق إن الكاتيجوري مش مضافة بالفعل لنفس المنتج
                var existingProductCategories = await _productCategoryRepository.GetProductCategoriesAsync(productId);
                var alreadyLinkedIds = existingProductCategories
                    .Select(c => c.Id)
                    .Intersect(categoryIds)
                    .ToList();

                if (alreadyLinkedIds.Any())
                {
                    return Response<bool>.Fail($"These categories are already linked: {string.Join(", ", alreadyLinkedIds)}");
                }

                //   الإضافة الفعلية
                await _productCategoryRepository.AddCategoriesToProductAsync(productId, categoryIds);

                return Response<bool>.Success(true, "Categories added to product successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error adding categories to product: {ex.Message}");
            }
        }

        public async Task<Response<bool>> UpdateProductCategoriesAsync(int productId, List<int> categoryIds)
        {
            try
            {
                await _productCategoryRepository.UpdateProductCategoriesAsync(productId, categoryIds);
                return Response<bool>.Success(true, "Product categories updated successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error updating product categories: {ex.Message}");
            }
        }

        public async Task<Response<List<CategoryDto>>> GetProductCategoriesAsync(int productId)
        {
            try
            {
                var categories = await _productCategoryRepository.GetProductCategoriesAsync(productId);

                if (categories?.Any() != true)
                {
                    return Response<List<CategoryDto>>.Fail("productnotfound");
                }
                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.Name,
                    ProductsCount = c.Products?.Count ?? 0,


                }).ToList();

                return Response<List<CategoryDto>>.Success(categoryDtos, "Product categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<CategoryDto>>.Fail($"Error retrieving product categories: {ex.Message}");
            }
        }


        public async Task<Response<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _productCategoryRepository.GetProductsByCategoryAsync(categoryId);

                if (products?.Any() != true)
                {

                    return Response<List<ProductDto>>.Fail("ProductsByCategorynotfound");


                }

                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    FinalPrice = p.FinalPrice,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CategoryName = p.Category.Name,
                    VendorShopName = p.Vendor?.ShopName,
                    DiscountPercent = p.DiscountPercent,
                    SKU = p.SKU,
                    StockQuantity = p.StockQuantity
                }).ToList();

                return Response<List<ProductDto>>.Success(productDtos, "Category products retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<ProductDto>>.Fail($"Error retrieving category products: {ex.Message}");
            }
        }


    }
}