using ECommerce.Core.DTOs.CartItem;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICurrentUserService _currentUserService;

        public CartController(ICartService cartService, ICurrentUserService currentUserService)
        {
            _cartService = cartService;
            _currentUserService = currentUserService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<Response<CartResponseDto>>> GetCart()
        {
            var result = await _cartService.GetUserCartAsync(_currentUserService.UserId);
            return Ok(result);
        }

        [HttpGet("GetCartSummary")]
        public async Task<ActionResult<Response<CartSummaryDto>>> GetCartSummary()
        {
            var result = await _cartService.GetCartSummaryAsync(_currentUserService.UserId);
            return Ok(result);
        }

        [HttpGet("GetCartItemsCount")]
        public async Task<ActionResult<Response<int>>> GetCartItemsCount()
        {
            var result = await _cartService.GetCartItemsCountAsync(_currentUserService.UserId);
            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Response<CartItemDto>>> AddToCart(AddToCartDto addToCartDto)
        {
            var result = await _cartService.AddToCartAsync(_currentUserService.UserId, addToCartDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("UpdateCartItem")]
        public async Task<ActionResult<Response<CartItemDto>>> UpdateCartItem(int cartItemId, UpdateCartItemDto updateCartDto)
        {
            var result = await _cartService.UpdateCartItemAsync(_currentUserService.UserId, cartItemId, updateCartDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("RemoveFromCart")]
        public async Task<ActionResult<Response<bool>>> RemoveFromCart(int cartItemId)
        {
            var result = await _cartService.RemoveFromCartAsync(_currentUserService.UserId, cartItemId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<Response<bool>>> ClearCart()
        {
            var result = await _cartService.ClearCartAsync(_currentUserService.UserId);
            return Ok(result);
        }

        [HttpPost("MergeCarts")]
        public async Task<ActionResult<Response<bool>>> MergeCarts(List<AddToCartDto> cartItems)
        {
            var result = await _cartService.MergeCartsAsync(_currentUserService.UserId, cartItems);
            return Ok(result);
        }
    }
}