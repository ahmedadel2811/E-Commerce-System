using ECommerce.Core.DTOs.order;

namespace ECommerce.Core.DTOs.shipment
{
    public class ShipmentTrackingDto
    {
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public string CourierName { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public OrderDto Order { get; set; }
        public string DeliveryAgentName { get; set; }
        public string DeliveryAgentPhone { get; set; }
    }
}
