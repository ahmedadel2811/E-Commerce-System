using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Repositories
{
    public class VendorRepository : GenericRepository<Vendor>, IVendorRepository
    {
        public VendorRepository(AppDbContext context) : base(context) { }

        public async Task<Vendor> GetVendorWithProductsAsync(int id)
        {
            return await _context.Vendors
                .Include(v => v.User)
                .Include(v => v.Products)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vendor> GetVendorByUserIdAsync(string userId)
        {
            return await _context.Vendors
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public async Task<IEnumerable<Vendor>> GetApprovedVendorsAsync()
        {
            return await _context.Vendors
                .Include(v => v.User)
                .Where(v => v.IsApproved)
                .ToListAsync();
        }

        public async Task<bool> VendorExistsAsync(string userId)
        {
            return await _context.Vendors
                .AnyAsync(v => v.UserId == userId);
        }




        public async Task<bool> UpdateVendorSales(int vendorId)
        {
            try
            {
                var orderItems = await _context.OrderItems
                    .Include(oi => oi.Product)
                    .Include(oi => oi.Order)
                    .Where(oi => oi.Product.VendorId == vendorId && oi.Order.Status==OrderStatus.Delivered)
                    .ToListAsync();

                var totalSales = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

                var vendor = await _context.Vendors.FindAsync(vendorId);
                if (vendor != null)
                {
                    vendor.TotalSales = totalSales;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the error
                return false;
            }
        }
        public async Task<decimal> GetVendorTotalSalesAsync(int vendorId)
        {
            var totalSales = await _context.OrderItems
       .Include(oi => oi.Product)
       .Include(oi => oi.Order)
       .Where(oi => oi.Product.VendorId == vendorId && oi.Order.Status==OrderStatus.Delivered)
       .Select(oi => oi.Quantity * oi.UnitPrice)
       .SumAsync();

            return totalSales;
        }
    }
}
