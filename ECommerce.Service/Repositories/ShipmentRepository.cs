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
    public class ShipmentRepository : GenericRepository<Shipment>, IShipmentRepository
    {
        public ShipmentRepository(AppDbContext context) : base(context) { }

        public async Task<Shipment> GetShipmentWithDetailsAsync(int id)
        {
            return await _context.Shipments
                .Include(s => s.Order)
                    .ThenInclude(o => o.User)
                .Include(s => s.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(s => s.DeliveryAgent)
                    .ThenInclude(da => da.User)
                .Include(s => s.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Shipment> GetShipmentByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.Shipments
                .Include(s => s.Order)
                    .ThenInclude(o => o.User)
                .Include(s => s.DeliveryAgent)
                .Include(s => s.OrderItems)
                .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByStatusAsync(string status)
        {
            var query = _context.Shipments
                .Include(s => s.Order)
                .Include(s => s.DeliveryAgent)
                .AsQueryable();

            // تحليل الحالة بناءً على التواريخ
            switch (status.ToLower())
            {
                case "pending":
                    query = query.Where(s => s.ShippedDate == null && s.DeliveredDate == null);
                    break;
                case "shipped":
                    query = query.Where(s => s.ShippedDate != null && s.DeliveredDate == null);
                    break;
                case "delivered":
                    query = query.Where(s => s.DeliveredDate != null);
                    break;
                case "in-transit":
                    query = query.Where(s => s.ShippedDate != null && s.DeliveredDate == null && s.EstimatedDelivery > DateTime.UtcNow);
                    break;
            }

            return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByDeliveryAgentAsync(int deliveryAgentId)
        {
            return await _context.Shipments
                .Include(s => s.Order)
                    .ThenInclude(o => o.User)
                .Include(s => s.OrderItems)
                .Where(s => s.DeliveryAgentId == deliveryAgentId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shipment>> GetPendingShipmentsAsync()
        {
            return await _context.Shipments
                .Include(s => s.Order)
                .Include(s => s.OrderItems)
                .Where(s => s.ShippedDate == null && s.DeliveredDate == null)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateShipmentStatusAsync(int shipmentId, DateTime? shippedDate, DateTime? deliveredDate, string trackingNumber = null)
        {
            var shipment = await _context.Shipments.FindAsync(shipmentId);
            if (shipment != null)
            {
                if (shippedDate.HasValue)
                    shipment.ShippedDate = shippedDate.Value;

                if (deliveredDate.HasValue)
                    shipment.DeliveredDate = deliveredDate.Value;

                if (!string.IsNullOrEmpty(trackingNumber))
                    shipment.TrackingNumber = trackingNumber;

                shipment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignDeliveryAgentAsync(int shipmentId, int deliveryAgentId)
        {
            var shipment = await _context.Shipments.FindAsync(shipmentId);
            if (shipment != null)
            {
                shipment.DeliveryAgentId = deliveryAgentId;
                shipment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TrackingNumberExistsAsync(string trackingNumber)
        {
            return await _context.Shipments
                .AnyAsync(s => s.TrackingNumber == trackingNumber);
        }
    }
}