namespace ECommerce.Core.DTOs.shipment
{
    public class UpdateShipmentDto
    {
        public string CourierName { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public int? DeliveryAgentId { get; set; }
    }
}
