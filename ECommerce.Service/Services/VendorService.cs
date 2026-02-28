using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.vendor;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICurrentUserService _currentUserService;

        public VendorService(
            IVendorRepository vendorRepository,
            ICurrentUserService currentUserService)
        {
            _vendorRepository = vendorRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Response<VendorDto>> GetVendorByIdAsync(int id)
        {
            try
            {
                var vendor = await _vendorRepository.GetVendorWithProductsAsync(id);
                if (vendor == null)
                    return Response<VendorDto>.Fail("Vendor not found");

                var vendorDto = MapToVendorDto(vendor);
                return Response<VendorDto>.Success(vendorDto, "Vendor retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<VendorDto>.Fail($"Error retrieving vendor: {ex.Message}");
            }
        }

        public async Task<Response<VendorDto>> GetVendorByUserIdAsync(string userId)
        {
            try
            {
                var vendor = await _vendorRepository.GetVendorByUserIdAsync(userId);
                if (vendor == null)
                    return Response<VendorDto>.Fail("Vendor not found");

                var vendorDto = MapToVendorDto(vendor);
                return Response<VendorDto>.Success(vendorDto, "Vendor retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<VendorDto>.Fail($"Error retrieving vendor: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<VendorDto>>> GetApprovedVendorsAsync()
        {
            try
            {
                var vendors = await _vendorRepository.GetApprovedVendorsAsync();
                var vendorDtos = vendors.Select(MapToVendorDto).ToList();
                return Response<IEnumerable<VendorDto>>.Success(vendorDtos, "Approved vendors retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<VendorDto>>.Fail($"Error retrieving approved vendors: {ex.Message}");
            }
        }

        public async Task<Response<VendorDto>> CreateVendorAsync(string userId, CreateVendorDto vendorDto)
        {
            try
            {
                var existingVendor = await _vendorRepository.VendorExistsAsync(userId);
                if (existingVendor)
                    return Response<VendorDto>.Fail("You already have a vendor account");

                var vendor = new Vendor
                {
                    UserId = userId,
                    ShopName = vendorDto.ShopName,
                    Description = vendorDto.Description,
                    LogoUrl = vendorDto.LogoUrl,
                    IsApproved = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createdVendor = await _vendorRepository.AddAsync(vendor);
                var resultDto = MapToVendorDto(createdVendor);
                return Response<VendorDto>.Success(resultDto, "Vendor application submitted successfully");
            }
            catch (Exception ex)
            {
                return Response<VendorDto>.Fail($"Error creating vendor: {ex.Message}");
            }
        }

        public async Task<Response<VendorDto>> UpdateVendorAsync(int id, string userId, UpdateVendorDto vendorDto)
        {
            try
            {
                var existingVendor = await _vendorRepository.GetByIdAsync(id);
                if (existingVendor == null)
                    return Response<VendorDto>.Fail("Vendor not found");

                if (existingVendor.UserId != userId)
                    return Response<VendorDto>.Fail("Unauthorized to update this vendor");

                existingVendor.ShopName = vendorDto.ShopName;
                existingVendor.Description = vendorDto.Description;
                existingVendor.LogoUrl = vendorDto.LogoUrl;
                existingVendor.UpdatedAt = DateTime.UtcNow;

                await _vendorRepository.UpdateAsync(existingVendor);
                var updatedDto = MapToVendorDto(existingVendor);
                return Response<VendorDto>.Success(updatedDto, "Vendor updated successfully");
            }
            catch (Exception ex)
            {
                return Response<VendorDto>.Fail($"Error updating vendor: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteVendorAsync(int id, string userId)
        {
            try
            {
                var vendor = await _vendorRepository.GetByIdAsync(id);
                if (vendor == null)
                    return Response<bool>.Fail("Vendor not found");

                if (vendor.UserId != userId && !_currentUserService.Roles.Contains("Admin"))
                    return Response<bool>.Fail("Unauthorized to delete this vendor");

                await _vendorRepository.DeleteAsync(vendor);
                return Response<bool>.Success(true, "Vendor deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error deleting vendor: {ex.Message}");
            }
        }

        public async Task<Response<bool>> ApproveVendorAsync(int id)
        {
            try
            {
                var vendor = await _vendorRepository.GetByIdAsync(id);
                if (vendor == null)
                    return Response<bool>.Fail("Vendor not found");

                vendor.IsApproved = true;
                vendor.UpdatedAt = DateTime.UtcNow;

                await _vendorRepository.UpdateAsync(vendor);
                return Response<bool>.Success(true, "Vendor approved successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error approving vendor: {ex.Message}");
            }
        }



        public async Task<Response<decimal>> GetVendorTotalSalesAsync(int vendorId)
        {
            try
            {
                var vendor = await _vendorRepository.GetByIdAsync(vendorId);
                if (vendor == null)
                    return Response<decimal>.Fail("Vendor not found");

                // التحقق من الصلاحيات  البائع يقدر يشوف مبيعاته فقط
                if (vendor.UserId != _currentUserService.UserId && !_currentUserService.Roles.Contains("Admin"))
                    return Response<decimal>.Fail("Unauthorized to view these sales");

                var totalSales = await _vendorRepository.GetVendorTotalSalesAsync(vendorId);
                return Response<decimal>.Success(totalSales, "Vendor total sales retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Fail($"Error retrieving vendor sales: {ex.Message}");
            }
        }

        public async Task<Response<bool>> UpdateVendorSalesAsync(int vendorId)
        {
            try
            {
                await _vendorRepository.UpdateVendorSales(vendorId);
                return Response<bool>.Success(true, "Vendor sales updated successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error updating vendor sales: {ex.Message}");
            }
        }

        private VendorDto MapToVendorDto(Vendor vendor)
        {
            return new VendorDto
            {
                Id = vendor.Id,
                UserId = vendor.UserId,
                ShopName = vendor.ShopName,
                Description = vendor.Description,
                LogoUrl = vendor.LogoUrl,
                IsApproved = vendor.IsApproved,
                RatingAverage = vendor.RatingAverage,
                TotalSales = vendor.TotalSales,
                UserName = vendor.User?.FullName,
                CreatedAt = vendor.CreatedAt
            };
        }
    }
}
