using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.order
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FinalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; }
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
        //public PaymentDto Payment { get; set; }
        //public ShipmentDto Shipment { get; set; }
    }
}