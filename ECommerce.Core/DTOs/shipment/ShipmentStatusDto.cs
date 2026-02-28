namespace ECommerce.Core.DTOs.shipment
{
    public class ShipmentStatusDto
    {
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string TrackingNumber { get; set; }
    }
}
