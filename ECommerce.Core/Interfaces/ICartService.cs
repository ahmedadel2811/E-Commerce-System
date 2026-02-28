using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.CartItem;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface ICartService
    {
        Task<Response<CartResponseDto>> GetUserCartAsync(string userId);
        Task<Response<CartItemDto>> AddToCartAsync(string userId, AddToCartDto addToCartDto);
        Task<Response<CartItemDto>> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto updateCartDto);
        Task<Response<bool>> RemoveFromCartAsync(string userId, int cartItemId);
        Task<Response<bool>> ClearCartAsync(string userId);
        Task<Response<CartSummaryDto>> GetCartSummaryAsync(string userId);
        Task<Response<int>> GetCartItemsCountAsync(string userId);
        Task<Response<bool>> MergeCartsAsync(string userId, List<AddToCartDto> cartItems);
    }
}