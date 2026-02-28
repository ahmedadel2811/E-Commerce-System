using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Repositories
{
    public class ProductCategoryRepository : GenericRepository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(AppDbContext context) : base(context) { }

        public async Task AddCategoriesToProductAsync(int productId, List<int> categoryIds)
        {
            var existingCategories = await _context.ProductCategories
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.CategoryId)
                .ToListAsync();

            var newCategories = categoryIds.Except(existingCategories)
                .Select(categoryId => new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = categoryId
                });

            await _context.ProductCategories.AddRangeAsync(newCategories);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCategoriesFromProductAsync(int productId, List<int> categoryIds)
        {
            var productCategories = await _context.ProductCategories
                .Where(pc => pc.ProductId == productId && categoryIds.Contains(pc.CategoryId))
                .ToListAsync();

            _context.ProductCategories.RemoveRange(productCategories);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductCategoriesAsync(int productId, List<int> categoryIds)
        {
            // Remove existing categories
            var existingCategories = await _context.ProductCategories
                .Where(pc => pc.ProductId == productId)
                .ToListAsync();

            _context.ProductCategories.RemoveRange(existingCategories);

            // Add new categories
            var newProductCategories = categoryIds.Select(categoryId => new ProductCategory
            {
                ProductId = productId,
                CategoryId = categoryId
            });

            await _context.ProductCategories.AddRangeAsync(newProductCategories);
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetProductCategoryIdsAsync(int productId)
        {
            return await _context.ProductCategories
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.CategoryId)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var productCategories = await _context.ProductCategories
        .Where(pc => pc.CategoryId == categoryId)
        .Include(pc => pc.Product)
        .Include(pc => pc.Category)
        .ToListAsync();

            var products = productCategories.Select(pc =>
            {
                pc.Product.Category = pc.Category;
                return pc.Product;
            }).ToList();

            return products;
        }

        public async Task<List<Category>> GetProductCategoriesAsync(int productId)
        {
            var productCategories = await _context.ProductCategories
      .Where(pc => pc.ProductId == productId)
      .Include(pc => pc.Product)
      .Include(pc => pc.Category)
      .ToListAsync();

            //      
            var categories = productCategories.Select(pc =>
            {
                pc.Category.Products = new List<Product> { pc.Product };
                return pc.Category;
            }).ToList();

            return categories;
        }
    }
}