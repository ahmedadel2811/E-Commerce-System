using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Repositories
{
    public class DeliveryAgentRepository : GenericRepository<DeliveryAgent>, IDeliveryAgentRepository
    {
        public DeliveryAgentRepository(AppDbContext context) : base(context) { }

        public async Task<DeliveryAgent> GetDeliveryAgentWithDetailsAsync(int id)
        {
            return await _context.DeliveryAgents
                .Include(da => da.User)
                .Include(da => da.Shipments)
                    .ThenInclude(s => s.Order)
                        .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(da => da.Id == id);
        }

        public async Task<DeliveryAgent> GetDeliveryAgentByUserIdAsync(string userId)
        {
            return await _context.DeliveryAgents
                .Include(da => da.User)
                .Include(da => da.Shipments)
                .FirstOrDefaultAsync(da => da.UserId == userId);
        }

        public async Task<IEnumerable<DeliveryAgent>> GetAvailableDeliveryAgentsAsync()
        {
            return await _context.DeliveryAgents
                .Include(da => da.User)
                .Where(da => da.CurrentStatus == "Available")
                .OrderBy(da => da.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeliveryAgent>> GetBusyDeliveryAgentsAsync()
        {
            return await _context.DeliveryAgents
                .Include(da => da.User)
                .Where(da => da.CurrentStatus == "Busy")
                .OrderBy(da => da.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeliveryAgent>> GetDeliveryAgentsByStatusAsync(string status)
        {
            return await _context.DeliveryAgents
                .Include(da => da.User)
                .Where(da => da.CurrentStatus == status)
                .OrderBy(da => da.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateDeliveryAgentStatusAsync(int deliveryAgentId, string status)
        {
            var deliveryAgent = await _context.DeliveryAgents.FindAsync(deliveryAgentId);
            if (deliveryAgent != null)
            {
                deliveryAgent.CurrentStatus = status;
                deliveryAgent.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetActiveShipmentsCountAsync(int deliveryAgentId)
        {
            return await _context.Shipments
                .CountAsync(s => s.DeliveryAgentId == deliveryAgentId &&
                                s.DeliveredDate == null);
        }

        public async Task<bool> IsVehicleNumberUniqueAsync(string vehicleNumber, int? excludeId = null)
        {
            var query = _context.DeliveryAgents.Where(da => da.VehicleNumber == vehicleNumber);

            if (excludeId.HasValue)
                query = query.Where(da => da.Id != excludeId.Value);

            return !await query.AnyAsync();
        }
    }
}