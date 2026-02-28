using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.Coupon;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface ICouponService
    {
        Task<Response<CouponDto>> GetCouponByIdAsync(int id);
        Task<Response<CouponDto>> GetCouponByCodeAsync(string code);
        Task<Response<IEnumerable<CouponDto>>> GetAllCouponsAsync();
        Task<Response<IEnumerable<CouponDto>>> GetActiveCouponsAsync();
        Task<Response<CouponDto>> CreateCouponAsync(CreateCouponDto createDto);
        Task<Response<CouponDto>> UpdateCouponAsync(int id, UpdateCouponDto updateDto);
        Task<Response<bool>> DeleteCouponAsync(int id);
        Task<Response<CouponValidationResult>> ValidateCouponAsync(ApplyCouponDto applyDto);
        Task<Response<bool>> AddProductsToCouponAsync(int couponId, List<int> productIds);
        Task<Response<bool>> RemoveProductsFromCouponAsync(int couponId, List<int> productIds);
    }
}
