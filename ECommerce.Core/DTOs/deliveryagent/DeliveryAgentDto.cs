using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.shipment;

namespace ECommerce.Core.DTOs.deliveryagent
{
    public class DeliveryAgentDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string VehicleNumber { get; set; }
        public string CurrentStatus { get; set; } // Available / Busy / Offline
        public int ActiveShipmentsCount { get; set; } // عدد الشحنات ال معاه النشطه
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class DeliveryAgentDetailDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string VehicleNumber { get; set; }
        public string CurrentStatus { get; set; }
        public int ActiveShipmentsCount { get; set; }
        public int TotalShipmentsCount { get; set; }
        public List<ShipmentDto> ActiveShipments { get; set; } = new List<ShipmentDto>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateDeliveryAgentDto
    {
        public string UserId { get; set; }
        public string VehicleNumber { get; set; }
        public string CurrentStatus { get; set; } = "Available";
    }

    public class UpdateDeliveryAgentDto
    {
        public string VehicleNumber { get; set; }
        public string CurrentStatus { get; set; }
    }

    public class DeliveryAgentStatsDto
    {
        public int TotalAgents { get; set; }
        public int AvailableAgents { get; set; }
        public int BusyAgents { get; set; }
        public int OfflineAgents { get; set; }
        public int TotalActiveShipments { get; set; }
        public double AverageShipmentsPerAgent { get; set; }
    }

    public class AssignShipmentToAgentDto
    {
        public int ShipmentId { get; set; }
        public int DeliveryAgentId { get; set; }
    }
}