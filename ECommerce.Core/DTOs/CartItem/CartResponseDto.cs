namespace ECommerce.Core.DTOs.CartItem
{
    public class CartResponseDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}