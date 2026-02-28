namespace ECommerce.Core.DTOs.shipment
{
    public class CreateShipmentDto
    {
        public int OrderId { get; set; }
        public string CourierName { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public int? DeliveryAgentId { get; set; }
        public List<int>? OrderItemIds { get; set; } = new List<int>();
    }
}
