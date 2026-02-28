using ECommerce.Core.DTOs.Coupon;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("GetAllCoupons")]
        public async Task<ActionResult<Response<IEnumerable<CouponDto>>>> GetAllCoupons()
        {
            var result = await _couponService.GetAllCouponsAsync();

            if(!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("ActiveCoupons")]
        public async Task<ActionResult<Response<IEnumerable<CouponDto>>>> GetActiveCoupons()
        {
            var result = await _couponService.GetActiveCouponsAsync();

            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("GetCouponById")]
        public async Task<ActionResult<Response<CouponDto>>> GetCouponById(int id)
        {
            var result = await _couponService.GetCouponByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetCouponByCode")]
        public async Task<ActionResult<Response<CouponDto>>> GetCouponByCode(string code)
        {
            var result = await _couponService.GetCouponByCodeAsync(code);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("CreateCoupon")]
        public async Task<ActionResult<Response<CouponDto>>> CreateCoupon(CreateCouponDto createDto)
        {
            var result = await _couponService.CreateCouponAsync(createDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetCouponById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<CouponDto>>> UpdateCoupon(int id, UpdateCouponDto updateDto)
        {
            var result = await _couponService.UpdateCouponAsync(id, updateDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("validate")]
        public async Task<ActionResult<Response<CouponValidationResult>>> ValidateCoupon(ApplyCouponDto applyDto)
        {
            var result = await _couponService.ValidateCouponAsync(applyDto);
            return Ok(result);
        }

        [HttpPost("{couponId}/products")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> AddProductsToCoupon(int couponId, [FromBody] List<int> productIds)
        {
            var result = await _couponService.AddProductsToCouponAsync(couponId, productIds);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{couponId}/products")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> RemoveProductsFromCoupon(int couponId, [FromBody] List<int> productIds)
        {
            var result = await _couponService.RemoveProductsFromCouponAsync(couponId, productIds);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}