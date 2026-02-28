using ECommerce.Core.DTOs.vendor;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly ICurrentUserService _currentUserService;

        public VendorsController(IVendorService vendorService, ICurrentUserService currentUserService)
        {
            _vendorService = vendorService;
            _currentUserService = currentUserService;
        }

        [HttpGet("GetVendors")]
        public async Task<ActionResult<Response<IEnumerable<VendorDto>>>> GetVendors()
        {
            var result = await _vendorService.GetApprovedVendorsAsync();
            return Ok(result);
        }

        [HttpGet("GetVendorById")]
        public async Task<ActionResult<Response<VendorDto>>> GetVendor(int id)
        {
            var result = await _vendorService.GetVendorByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetMyVendor")]
        [Authorize]
        public async Task<ActionResult<Response<VendorDto>>> GetMyVendor()
        {
            var result = await _vendorService.GetVendorByUserIdAsync(_currentUserService.UserId);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("CreateVendor")]
        [Authorize]
        public async Task<ActionResult<Response<VendorDto>>> CreateVendor(CreateVendorDto vendorDto)
        {
            var result = await _vendorService.CreateVendorAsync(_currentUserService.UserId, vendorDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetVendor), new { id = result.Data.Id }, result);
        }

        [HttpPut("UpdateVendor")]
        [Authorize]
        public async Task<ActionResult<Response<VendorDto>>> UpdateVendor(int id, UpdateVendorDto vendorDto)
        {
            var result = await _vendorService.UpdateVendorAsync(id, _currentUserService.UserId, vendorDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("DeleteVendor")]
        [Authorize]
        public async Task<ActionResult<Response<bool>>> DeleteVendor(int id)
        {
            var result = await _vendorService.DeleteVendorAsync(id, _currentUserService.UserId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ApproveVendor")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> ApproveVendor(int id)
        {
            var result = await _vendorService.ApproveVendorAsync(id);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpGet("GetVendorSales")]
        [Authorize]
        public async Task<ActionResult<Response<decimal>>> GetVendorSales(int id)
        {
            var result = await _vendorService.GetVendorTotalSalesAsync(id);
            return Ok(result);
        }


        [HttpGet("profile/my/sales")]
        [Authorize]
        public async Task<ActionResult<Response<decimal>>> GetMyVendorSales()
        {
            var vendorResult = await _vendorService.GetVendorByUserIdAsync(_currentUserService.UserId);
            if (!vendorResult.Succeeded)
                return NotFound(vendorResult);

            var result = await _vendorService.GetVendorTotalSalesAsync(vendorResult.Data.Id);
            return Ok(result);
        }

        [HttpPost("UpdateVendorSales")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> UpdateVendorSales(int id)
        {
            var result = await _vendorService.UpdateVendorSalesAsync(id);
            return Ok(result);
        }
    }
}