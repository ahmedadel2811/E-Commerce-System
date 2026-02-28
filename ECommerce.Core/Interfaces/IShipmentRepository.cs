using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IShipmentRepository : IGenericRepository<Shipment>
    {
        Task<Shipment> GetShipmentWithDetailsAsync(int id);
        Task<Shipment> GetShipmentByTrackingNumberAsync(string trackingNumber);
        Task<IEnumerable<Shipment>> GetShipmentsByStatusAsync(string status);
        Task<IEnumerable<Shipment>> GetShipmentsByDeliveryAgentAsync(int deliveryAgentId);
        Task<IEnumerable<Shipment>> GetPendingShipmentsAsync();
        Task UpdateShipmentStatusAsync(int shipmentId, DateTime? shippedDate, DateTime? deliveredDate, string trackingNumber = null);
        Task AssignDeliveryAgentAsync(int shipmentId, int deliveryAgentId);
        Task<bool> TrackingNumberExistsAsync(string trackingNumber);
    }
}
