namespace ECommerce.Core.DTOs.CartItem
{
    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}