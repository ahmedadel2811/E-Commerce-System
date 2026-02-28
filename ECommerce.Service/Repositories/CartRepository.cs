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
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CartItem>> GetUserCartAsync(string userId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Images)
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.Vendor)
                .Where(ci => ci.UserId == userId)
                .OrderByDescending(ci => ci.CreatedAt)
                .ToListAsync();
        }

        public async Task<CartItem> GetCartItemAsync(string userId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        public async Task ClearUserCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            return await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Quantity * ci.UnitPrice);
        }

        public async Task<int> GetCartItemsCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Quantity);
        }

        public async Task<bool> CartItemExistsAsync(string userId, int productId)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }
    }
}