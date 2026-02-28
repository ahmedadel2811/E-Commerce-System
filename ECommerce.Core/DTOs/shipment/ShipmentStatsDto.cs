namespace ECommerce.Core.DTOs.shipment
{
    public class ShipmentStatsDto
    {
        public int TotalShipments { get; set; }
        public int PendingShipments { get; set; }
        public int ShippedShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int LateShipments { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public int? InTransitShipments { get; set; }
        public decimal? DeliverySuccessRate { get; set; }
        public double? AverageDeliveryTimeHours { get; set; }
    }
}
