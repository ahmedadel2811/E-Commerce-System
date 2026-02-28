using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs.shipment
{
    public class ShipmentDto
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string? OrderNumber { get; set; }
        public string CourierName { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? Status { get; set; }
        public int? DeliveryAgentId { get; set; }
        public string? DeliveryAgentName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
