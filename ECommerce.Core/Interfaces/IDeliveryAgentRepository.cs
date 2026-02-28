using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IDeliveryAgentRepository : IGenericRepository<DeliveryAgent>
    {
        Task<DeliveryAgent> GetDeliveryAgentWithDetailsAsync(int id);
        Task<DeliveryAgent> GetDeliveryAgentByUserIdAsync(string userId);
        Task<IEnumerable<DeliveryAgent>> GetAvailableDeliveryAgentsAsync();
        Task<IEnumerable<DeliveryAgent>> GetBusyDeliveryAgentsAsync();
        Task<IEnumerable<DeliveryAgent>> GetDeliveryAgentsByStatusAsync(string status);
        Task UpdateDeliveryAgentStatusAsync(int deliveryAgentId, string status);
        Task<int> GetActiveShipmentsCountAsync(int deliveryAgentId);
        Task<bool> IsVehicleNumberUniqueAsync(string vehicleNumber, int? excludeId = null);
    }
}
