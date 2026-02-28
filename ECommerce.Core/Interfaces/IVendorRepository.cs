using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IVendorRepository : IGenericRepository<Vendor>
    {
        Task<Vendor> GetVendorWithProductsAsync(int id);
        Task<Vendor> GetVendorByUserIdAsync(string userId);
        Task<IEnumerable<Vendor>> GetApprovedVendorsAsync();
        Task<bool> VendorExistsAsync(string userId);


        Task<bool> UpdateVendorSales(int vendorId);
        Task<decimal> GetVendorTotalSalesAsync(int vendorId);

    }
}
