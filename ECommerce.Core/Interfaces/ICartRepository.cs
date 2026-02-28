using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface ICartRepository : IGenericRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetUserCartAsync(string userId);
        Task<CartItem> GetCartItemAsync(string userId, int productId);
        Task ClearUserCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task<int> GetCartItemsCountAsync(string userId);
        Task<bool> CartItemExistsAsync(string userId, int productId);
    }
}
