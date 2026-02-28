using ECommerce.Core.DTOs.order;

namespace ECommerce.Core.DTOs.shipment
{
    public class ShipmentDetailDto
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string CourierName { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string Status { get; set; }
        public int? DeliveryAgentId { get; set; }
        public string DeliveryAgentName { get; set; }
        public string DeliveryAgentPhone { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderDto Order { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
