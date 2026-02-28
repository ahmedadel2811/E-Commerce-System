using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.Coupon;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IProductRepository _productRepository;

        public CouponService(ICouponRepository couponRepository, IProductRepository productRepository)
        {
            _couponRepository = couponRepository;
            _productRepository = productRepository;
        }

        public async Task<Response<CouponDto>> GetCouponByIdAsync(int id)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(id);
                if (coupon == null)
                    return Response<CouponDto>.Fail("Coupon not found");

                var couponDto = MapToDto(coupon);
                return Response<CouponDto>.Success(couponDto, "Coupon retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<CouponDto>.Fail($"Error retrieving coupon: {ex.Message}");
            }
        }

        public async Task<Response<CouponDto>> GetCouponByCodeAsync(string code)
        {
            try
            {
                var coupon = await _couponRepository.GetByCodeAsync(code);
                if (coupon == null)
                    return Response<CouponDto>.Fail("Coupon not found");

                var couponDto = MapToDto(coupon);
                return Response<CouponDto>.Success(couponDto, "Coupon retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<CouponDto>.Fail($"Error retrieving coupon: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<CouponDto>>> GetAllCouponsAsync()
        {
            try
            {
                var coupons = await _couponRepository.GetAllAsync();
                if(coupons == null)
                {
                    return Response<IEnumerable<CouponDto>>.Fail("No Coupons");

                }
                var couponDtos = coupons.Select(MapToDto).ToList();
                return Response<IEnumerable<CouponDto>>.Success(couponDtos, "Coupons retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<CouponDto>>.Fail($"Error retrieving coupons: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<CouponDto>>> GetActiveCouponsAsync()
        {
            try
            {
                var coupons = await _couponRepository.GetActiveCouponsAsync();
                if(coupons?.Any()!=true)
                {
                    return Response<IEnumerable<CouponDto>>.Fail("no active coupons");
                }
                var couponDtos = coupons.Select(MapToDto).ToList();
                return Response<IEnumerable<CouponDto>>.Success(couponDtos, "Active coupons retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<CouponDto>>.Fail($"Error retrieving active coupons: {ex.Message}");
            }
        }

        public async Task<Response<CouponDto>> CreateCouponAsync(CreateCouponDto createDto)
        {
            try
            {
                // Check if code already exists
                if (await _couponRepository.CodeExistsAsync(createDto.Code))
                    return Response<CouponDto>.Fail("Coupon code already exists");

                // Validate discount type
                if (!createDto.DiscountAmount.HasValue && !createDto.DiscountPercent.HasValue)
                    return Response<CouponDto>.Fail("Either discount amount or discount percent must be provided");

                if (createDto.DiscountAmount.HasValue && createDto.DiscountPercent.HasValue)
                    return Response<CouponDto>.Fail("Cannot provide both discount amount and discount percent");

                // Create coupon
                var coupon = new Coupon
                {
                    Code = createDto.Code.ToUpper(),
                    DiscountAmount = createDto.DiscountAmount,
                    DiscountPercent = createDto.DiscountPercent,
                    ExpiryDate = createDto.ExpiryDate,
                    UsageLimit = createDto.UsageLimit,
                    IsActive = createDto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var createdCoupon = await _couponRepository.AddAsync(coupon);

                // Add products to coupon if specified
                if (createDto.ProductIds != null && createDto.ProductIds.Any())
                {
                    await _couponRepository.AddProductsToCouponAsync(createdCoupon.Id, createDto.ProductIds);
                }

                // Reload coupon with products
                var fullCoupon = await _couponRepository.GetByIdAsync(createdCoupon.Id);
                var couponDto = MapToDto(fullCoupon);

                return Response<CouponDto>.Success(couponDto, "Coupon created successfully");
            }
            catch (Exception ex)
            {
                return Response<CouponDto>.Fail($"Error creating coupon: {ex.Message}");
            }
        }

        public async Task<Response<CouponDto>> UpdateCouponAsync(int id, UpdateCouponDto updateDto)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(id);
                if (coupon == null)
                    return Response<CouponDto>.Fail("Coupon not found");

                // Check if code already exists (excluding current coupon)
                if (!string.IsNullOrEmpty(updateDto.Code) && await _couponRepository.CodeExistsAsync(updateDto.Code, id))
                    return Response<CouponDto>.Fail("Coupon code already exists");

                // Update properties
                if (!string.IsNullOrEmpty(updateDto.Code))
                    coupon.Code = updateDto.Code.ToUpper();

                if (updateDto.DiscountAmount.HasValue || updateDto.DiscountPercent.HasValue)
                {
                    if (updateDto.DiscountAmount.HasValue && updateDto.DiscountPercent.HasValue)
                        return Response<CouponDto>.Fail("Cannot provide both discount amount and discount percent");

                    coupon.DiscountAmount = updateDto.DiscountAmount;
                    coupon.DiscountPercent = updateDto.DiscountPercent;
                }

                if (updateDto.ExpiryDate.HasValue)
                    coupon.ExpiryDate = updateDto.ExpiryDate.Value;

                if (updateDto.UsageLimit.HasValue)
                    coupon.UsageLimit = updateDto.UsageLimit.Value;

                if (updateDto.IsActive.HasValue)
                    coupon.IsActive = updateDto.IsActive.Value;

                coupon.UpdatedAt = DateTime.UtcNow;

                await _couponRepository.UpdateAsync(coupon);

                // Update products if specified
                if (updateDto.ProductIds != null)
                {
                    // Remove all current products and add new ones
                    var currentProducts = await _couponRepository.GetCouponProductsAsync(id);
                    var currentProductIds = currentProducts.Select(p => p.Id).ToList();

                    if (currentProductIds.Any())
                        await _couponRepository.RemoveProductsFromCouponAsync(id, currentProductIds);

                    if (updateDto.ProductIds.Any())
                        await _couponRepository.AddProductsToCouponAsync(id, updateDto.ProductIds);
                }

                // Reload coupon with products
                var updatedCoupon = await _couponRepository.GetByIdAsync(id);
                var couponDto = MapToDto(updatedCoupon);

                return Response<CouponDto>.Success(couponDto, "Coupon updated successfully");
            }
            catch (Exception ex)
            {
                return Response<CouponDto>.Fail($"Error updating coupon: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteCouponAsync(int id)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(id);
                if (coupon == null)
                    return Response<bool>.Fail("Coupon not found");

                await _couponRepository.DeleteAsync(coupon);
                return Response<bool>.Success(true, "Coupon deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error deleting coupon: {ex.Message}");
            }
        }

        public async Task<Response<CouponValidationResult>> ValidateCouponAsync(ApplyCouponDto applyDto)
        {
            try
            {
                var coupon = await _couponRepository.GetByCodeAsync(applyDto.Code);
                if (coupon == null)
                    return Response<CouponValidationResult>.Fail("Invalid coupon code");

                var validationResult = new CouponValidationResult { Coupon = MapToDto(coupon) };

                // Check if coupon is active
                if (!coupon.IsActive)
                {
                    validationResult.Message = "Coupon is not active";
                    return Response<CouponValidationResult>.Success(validationResult, "Coupon validation completed");
                }

                // Check if coupon is expired
                if (coupon.ExpiryDate < DateTime.UtcNow)
                {
                    validationResult.Message = "Coupon has expired";
                    return Response<CouponValidationResult>.Success(validationResult, "Coupon validation completed");
                }

                // Check if coupon has usage limit and is exceeded
                if (coupon.UsageLimit > 0)
                {
                    // Here you would typically check actual usage count from orders
                    // For now, we'll assume it's not exceeded
                }

                // Check if coupon applies to specific products
                var couponProducts = await _couponRepository.GetCouponProductsAsync(coupon.Id);
                if (couponProducts.Any())
                {
                    // If coupon has specific products, check if applied products match
                    if (!applyDto.ProductIds.Any() || !applyDto.ProductIds.Any(pid =>
                        couponProducts.Any(cp => cp.Id == pid)))
                    {
                        validationResult.Message = "Coupon does not apply to any of the selected products";
                        return Response<CouponValidationResult>.Success(validationResult, "Coupon validation completed");
                    }
                }

                // Calculate discount amount
                decimal discountAmount = 0;
                if (coupon.DiscountAmount.HasValue)
                {
                    discountAmount = coupon.DiscountAmount.Value;
                }
                else if (coupon.DiscountPercent.HasValue)
                {
                    discountAmount = applyDto.OrderTotal * (decimal)(coupon.DiscountPercent.Value / 100);
                }

                // Ensure discount doesn't exceed order total
                if (discountAmount > applyDto.OrderTotal)
                    discountAmount = applyDto.OrderTotal;

                validationResult.IsValid = true;
                validationResult.DiscountAmount = discountAmount;
                validationResult.Message = "Coupon applied successfully";

                return Response<CouponValidationResult>.Success(validationResult, "Coupon validation completed");
            }
            catch (Exception ex)
            {
                return Response<CouponValidationResult>.Fail($"Error validating coupon: {ex.Message}");
            }
        }

        public async Task<Response<bool>> AddProductsToCouponAsync(int couponId, List<int> productIds)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(couponId);
                if (coupon == null)
                    return Response<bool>.Fail("Coupon not found");

                // Validate products exist
                foreach (var productId in productIds)
                {
                    var product = await _productRepository.GetByIdAsync(productId);
                    if (product == null)
                        return Response<bool>.Fail($"Product with ID {productId} not found");
                }

                await _couponRepository.AddProductsToCouponAsync(couponId, productIds);
                return Response<bool>.Success(true, "Products added to coupon successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error adding products to coupon: {ex.Message}");
            }
        }

        public async Task<Response<bool>> RemoveProductsFromCouponAsync(int couponId, List<int> productIds)
        {
            try
            {
                var coupon = await _couponRepository.GetByIdAsync(couponId);
                if (coupon == null)
                    return Response<bool>.Fail("Coupon not found");

                await _couponRepository.RemoveProductsFromCouponAsync(couponId, productIds);
                return Response<bool>.Success(true, "Products removed from coupon successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error removing products from coupon: {ex.Message}");
            }
        }

        private CouponDto MapToDto(Coupon coupon)
        {
            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                DiscountAmount = coupon.DiscountAmount,
                DiscountPercent = coupon.DiscountPercent,
                ExpiryDate = coupon.ExpiryDate,
                UsageLimit = coupon.UsageLimit,
                IsActive = coupon.IsActive,
                ProductIds = coupon.ProductCoupons?.Select(pc => pc.ProductId).ToList() ?? new List<int>()
            };
        }
    }
}