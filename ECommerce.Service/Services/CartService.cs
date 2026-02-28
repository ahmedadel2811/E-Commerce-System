using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Core.DTOs.CartItem;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using static ECommerce.Service.Services.CartService;

namespace ECommerce.Service.Services
{

    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Response<CartResponseDto>> GetUserCartAsync(string userId)
        {
            try
            {
                var cartItems = await _cartRepository.GetUserCartAsync(userId);
                var cartItemDtos = cartItems.Select(MapToCartItemDto).ToList();

                var totalItems = await _cartRepository.GetCartItemsCountAsync(userId);
                var totalAmount = await _cartRepository.GetCartTotalAsync(userId);

                var response = new CartResponseDto
                {
                    Items = cartItemDtos,
                    TotalItems = totalItems,
                    TotalAmount = totalAmount
                };

                return Response<CartResponseDto>.Success(response, "Cart retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<CartResponseDto>.Fail($"Error retrieving cart: {ex.Message}");
            }
        }

        public async Task<Response<CartItemDto>> AddToCartAsync(string userId, AddToCartDto addToCartDto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
                if (product == null)
                    return Response<CartItemDto>.Fail("Product not found");

                if (!product.IsActive)
                    return Response<CartItemDto>.Fail("Product is not available");

                if (product.StockQuantity < addToCartDto.Quantity)
                    return Response<CartItemDto>.Fail($"Insufficient stock. Only {product.StockQuantity} items available");

                // Check if item already exists in cart
                var existingCartItem = await _cartRepository.GetCartItemAsync(userId, addToCartDto.ProductId);

                if (existingCartItem != null)
                {
                    // Update quantity if item exists
                    existingCartItem.Quantity += addToCartDto.Quantity;

                    if (existingCartItem.Quantity > product.StockQuantity)
                        return Response<CartItemDto>.Fail($"Cannot add more than available stock. Only {product.StockQuantity} items available");

                    await _cartRepository.UpdateAsync(existingCartItem);
                    var updatedDto = MapToCartItemDto(existingCartItem);
                    return Response<CartItemDto>.Success(updatedDto, "Cart item updated successfully");
                }
                else
                {
                    // Add new item to cart
                    var cartItem = new CartItem
                    {
                        UserId = userId,
                        ProductId = addToCartDto.ProductId,
                        Quantity = addToCartDto.Quantity,
                        UnitPrice = product.FinalPrice,
                        CreatedAt = DateTime.UtcNow
                    };

                    var createdCartItem = await _cartRepository.AddAsync(cartItem);
                    var cartItemDto = MapToCartItemDto(createdCartItem);
                    return Response<CartItemDto>.Success(cartItemDto, "Item added to cart successfully");
                }
            }
            catch (Exception ex)
            {
                return Response<CartItemDto>.Fail($"Error adding to cart: {ex.Message}");
            }
        }

        public async Task<Response<CartItemDto>> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemDto updateCartDto)
        {
            try
            {
                var cartItem = await _cartRepository.GetByIdAsync(cartItemId);
                if (cartItem == null)
                    return Response<CartItemDto>.Fail("Cart item not found");

                if (cartItem.UserId != userId)
                    return Response<CartItemDto>.Fail("Unauthorized to update this cart item");

                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                    return Response<CartItemDto>.Fail("Product not found");

                if (updateCartDto.Quantity <= 0)
                {
                    await _cartRepository.DeleteAsync(cartItem);
                    return Response<CartItemDto>.Success(null, "Cart item removed");
                }

                if (product.StockQuantity < updateCartDto.Quantity)
                    return Response<CartItemDto>.Fail($"Insufficient stock. Only {product.StockQuantity} items available");

                cartItem.Quantity = updateCartDto.Quantity;
                cartItem.UnitPrice = product.FinalPrice; // Update price in case it changed
                cartItem.UpdatedAt = DateTime.UtcNow;

                await _cartRepository.UpdateAsync(cartItem);
                var updatedDto = MapToCartItemDto(cartItem);
                return Response<CartItemDto>.Success(updatedDto, "Cart item updated successfully");
            }
            catch (Exception ex)
            {
                return Response<CartItemDto>.Fail($"Error updating cart item: {ex.Message}");
            }
        }

        public async Task<Response<bool>> RemoveFromCartAsync(string userId, int cartItemId)
        {
            try
            {
                var cartItem = await _cartRepository.GetByIdAsync(cartItemId);
                if (cartItem == null)
                    return Response<bool>.Fail("Cart item not found");

                if (cartItem.UserId != userId)
                    return Response<bool>.Fail("Unauthorized to remove this cart item");

                await _cartRepository.DeleteAsync(cartItem);
                return Response<bool>.Success(true, "Item removed from cart successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error removing from cart: {ex.Message}");
            }
        }

        public async Task<Response<bool>> ClearCartAsync(string userId)
        {
            try
            {
                await _cartRepository.ClearUserCartAsync(userId);
                return Response<bool>.Success(true, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error clearing cart: {ex.Message}");
            }
        }

        public async Task<Response<CartSummaryDto>> GetCartSummaryAsync(string userId)
        {
            try
            {
                var cartItems = await _cartRepository.GetUserCartAsync(userId);
                var cartItemDtos = cartItems.Select(MapToCartItemDto).ToList();

                var subTotal = await _cartRepository.GetCartTotalAsync(userId);
                var shippingFee = CalculateShippingFee(subTotal);
                var discount = 0m; // لو عندى كوبون هفعلها
                var total = subTotal + shippingFee - discount;

                var summary = new CartSummaryDto
                {
                    Items = cartItemDtos,
                    TotalItems = cartItemDtos.Sum(ci => ci.Quantity),
                    SubTotal = subTotal,
                    ShippingFee = shippingFee,
                    Discount = discount,
                    Total = total
                };

                return Response<CartSummaryDto>.Success(summary, "Cart summary retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<CartSummaryDto>.Fail($"Error retrieving cart summary: {ex.Message}");
            }
        }

        public async Task<Response<int>> GetCartItemsCountAsync(string userId)
        {
            try
            {
                var count = await _cartRepository.GetCartItemsCountAsync(userId);
                return Response<int>.Success(count, "Cart items count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<int>.Fail($"Error retrieving cart items count: {ex.Message}");
            }
        }

        public async Task<Response<bool>> MergeCartsAsync(string userId, List<AddToCartDto> cartItems)
        {
            try
            {
                foreach (var item in cartItems)
                {
                    await AddToCartAsync(userId, item);
                }
                return Response<bool>.Success(true, "Carts merged successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error merging carts: {ex.Message}");
            }
        }

        private decimal CalculateShippingFee(decimal subTotal)
        {
            if (subTotal >= 100)
                return 0;

            return 10m;
        }

        //  Mapping Method
        private CartItemDto MapToCartItemDto(CartItem cartItem)
        {
            return new CartItemDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product?.Name,
                ProductImage = cartItem.Product?.ImageUrl,
                ProductPrice = cartItem.Product?.Price ?? 0,
                FinalPrice = cartItem.Product?.FinalPrice ?? 0,
                VendorShopName = cartItem.Product?.Vendor?.ShopName,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                SubTotal = cartItem.SubTotal,
                StockQuantity = cartItem.Product?.StockQuantity ?? 0,
                IsActive = cartItem.Product?.IsActive ?? false
            };
        }
    }
}
