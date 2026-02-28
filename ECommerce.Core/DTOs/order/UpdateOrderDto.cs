using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.order
{
    public class UpdateOrderDto
    {
        public OrderStatus Status { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal ShippingFee { get; set; }
    }
}