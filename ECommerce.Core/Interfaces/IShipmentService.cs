using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.shipment;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IShipmentService
    {
        Task<Response<ShipmentDetailDto>> GetShipmentByIdAsync(int id);
        Task<Response<ShipmentTrackingDto>> GetShipmentByTrackingNumberAsync(string trackingNumber);
        Task<Response<IEnumerable<ShipmentDto>>> GetAllShipmentsAsync();
        Task<Response<IEnumerable<ShipmentDto>>> GetShipmentsByStatusAsync(string status);
        Task<Response<IEnumerable<ShipmentDto>>> GetShipmentsByDeliveryAgentAsync(int deliveryAgentId);
        Task<Response<IEnumerable<ShipmentDto>>> GetPendingShipmentsAsync();
        Task<Response<ShipmentDto>> CreateShipmentAsync(CreateShipmentDto shipmentDto);
        Task<Response<ShipmentDto>> UpdateShipmentAsync(int id, UpdateShipmentDto shipmentDto);
        Task<Response<ShipmentDto>> UpdateShipmentStatusAsync(int id, ShipmentStatusDto statusDto);
        Task<Response<bool>> AssignDeliveryAgentAsync(int shipmentId, int deliveryAgentId);
        Task<Response<bool>> MarkAsShippedAsync(int shipmentId, string trackingNumber = null);
        Task<Response<bool>> MarkAsDeliveredAsync(int shipmentId);
        Task<Response<ShipmentStatsDto>> GetShipmentStatsAsync();
        Task<Response<bool>> CanCreateShipmentAsync(int orderId);
    }
}
