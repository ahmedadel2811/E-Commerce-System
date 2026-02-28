using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.order
{
    public class CreateOrderDto
    {
        //public string UserId { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
        public decimal ShippingFee { get; set; }
        public int? AddressId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Wallet;
        public string? CouponCode { get; set; }
    }
}