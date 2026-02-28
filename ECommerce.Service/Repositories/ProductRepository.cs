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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<IQueryable<Product>> GetProductsWithCategoryAsync()
        {
            return  _context.Products
                                            .Include(p => p.Category)
                                            .Include(p => p.Vendor)
                                            .Where(p => p.IsActive);
        }

        public async Task<Product> GetProductWithDetailsAsync(int id)
        {
            return await _context.Products
.Include(p => p.Category)
.Include(p => p.Vendor)
.Include(p => p.Images)
.Include(p => p.Variants)
.Include(p => p.Reviews)
.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByVendorAsync(int vendorId)
        {
            return await _context.Products
                .Where(p => p.VendorId == vendorId && p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task UpdateProductStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity = quantity;
                await _context.SaveChangesAsync();
            }

           
        }
    }
}