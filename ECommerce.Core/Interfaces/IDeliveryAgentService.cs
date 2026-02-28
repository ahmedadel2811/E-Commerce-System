using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.deliveryagent;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IDeliveryAgentService
    {
        Task<Response<DeliveryAgentDetailDto>> GetDeliveryAgentByIdAsync(int id);
        Task<Response<DeliveryAgentDetailDto>> GetDeliveryAgentByUserIdAsync(string userId);
        Task<Response<DeliveryAgentDto>> GetCurrentDeliveryAgentAsync(string userId);
        Task<Response<IEnumerable<DeliveryAgentDto>>> GetAllDeliveryAgentsAsync();
        Task<Response<IEnumerable<DeliveryAgentDto>>> GetAvailableDeliveryAgentsAsync();
        Task<Response<IEnumerable<DeliveryAgentDto>>> GetDeliveryAgentsByStatusAsync(string status);
        Task<Response<DeliveryAgentDto>> CreateDeliveryAgentAsync(CreateDeliveryAgentDto deliveryAgentDto);
        Task<Response<DeliveryAgentDto>> UpdateDeliveryAgentAsync(int id, UpdateDeliveryAgentDto deliveryAgentDto);
        Task<Response<bool>> DeleteDeliveryAgentAsync(int id);
        Task<Response<DeliveryAgentDto>> UpdateDeliveryAgentStatusAsync(int id, string status);
        Task<Response<bool>> AssignShipmentToAgentAsync(int shipmentId, int deliveryAgentId);
        Task<Response<DeliveryAgentStatsDto>> GetDeliveryAgentStatsAsync();
        Task<Response<int>> GetActiveShipmentsCountAsync(int deliveryAgentId);
    }
}
