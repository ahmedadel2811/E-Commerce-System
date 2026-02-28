using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.vendor;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IVendorService
    {
        Task<Response<VendorDto>> GetVendorByIdAsync(int id);
        Task<Response<VendorDto>> GetVendorByUserIdAsync(string userId);
        Task<Response<IEnumerable<VendorDto>>> GetApprovedVendorsAsync();
        Task<Response<VendorDto>> CreateVendorAsync(string userId, CreateVendorDto vendorDto);
        Task<Response<VendorDto>> UpdateVendorAsync(int id, string userId, UpdateVendorDto vendorDto);
        Task<Response<bool>> DeleteVendorAsync(int id, string userId);
        Task<Response<bool>> ApproveVendorAsync(int id);


        Task<Response<decimal>> GetVendorTotalSalesAsync(int vendorId);

        Task<Response<bool>> UpdateVendorSalesAsync(int vendorId);


    }
}
