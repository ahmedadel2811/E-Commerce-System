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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetCategoriesWithSubCategoriesAsync()
        {
            return await _context.Categories.Include(x => x.Products)
                .Include(c => c.SubCategories)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryWithProductsAsync(int id)
        {
            return await _context.Categories
                                .Include(c => c.SubCategories).ThenInclude(c => c.Products).Include(c => c.Products)
               .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetMainCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategoryId).Include(x=> x.Products)
                .ToListAsync();
        }

        public async Task<bool> CategoryHasProductsAsync(int categoryId)
        {
            return await _context.Products
                .AnyAsync(p => p.CategoryId == categoryId ||
                              p.ProductCategories.Any(pc => pc.CategoryId == categoryId));
        }
    }
}