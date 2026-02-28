using ECommerce.Core.DTOs.shipment;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly ICurrentUserService _currentUserService;

        public ShipmentsController(IShipmentService shipmentService, ICurrentUserService currentUserService)
        {
            _shipmentService = shipmentService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<IEnumerable<ShipmentDto>>>> GetShipments()
        {
            var result = await _shipmentService.GetAllShipmentsAsync();
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<Response<IEnumerable<ShipmentDto>>>> GetMyShipments()
        {
            // المستخدم العادي بيشوف الشحنات بتاعته بس
            var result = await _shipmentService.GetShipmentsByStatusAsync("all");
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Response<ShipmentDetailDto>>> GetShipment(int id)
        {
            var result = await _shipmentService.GetShipmentByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("tracking/{trackingNumber}")]
        [AllowAnonymous] // أي حد يقدر يتتبع الشحنة من غير ما يسجل دخول
        public async Task<ActionResult<Response<ShipmentTrackingDto>>> TrackShipment(string trackingNumber)
        {
            var result = await _shipmentService.GetShipmentByTrackingNumberAsync(trackingNumber);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<IEnumerable<ShipmentDto>>>> GetShipmentsByStatus(string status)
        {
            var result = await _shipmentService.GetShipmentsByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<IEnumerable<ShipmentDto>>>> GetPendingShipments()
        {
            var result = await _shipmentService.GetPendingShipmentsAsync();
            return Ok(result);
        }

        [HttpGet("delivery-agent/{deliveryAgentId}")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<IEnumerable<ShipmentDto>>>> GetShipmentsByDeliveryAgent(int deliveryAgentId)
        {
            var result = await _shipmentService.GetShipmentsByDeliveryAgentAsync(deliveryAgentId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<ShipmentDto>>> CreateShipment(CreateShipmentDto shipmentDto)
        {
            var result = await _shipmentService.CreateShipmentAsync(shipmentDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetShipment), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<ShipmentDto>>> UpdateShipment(int id, UpdateShipmentDto shipmentDto)
        {
            var result = await _shipmentService.UpdateShipmentAsync(id, shipmentDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<ShipmentDto>>> UpdateShipmentStatus(int id, ShipmentStatusDto statusDto)
        {
            var result = await _shipmentService.UpdateShipmentStatusAsync(id, statusDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{id}/assign-agent")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> AssignDeliveryAgent(int id, [FromBody] AssignDeliveryAgentDto assignDto)
        {
            var result = await _shipmentService.AssignDeliveryAgentAsync(id, assignDto.DeliveryAgentId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{id}/mark-shipped")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<bool>>> MarkAsShipped(int id, [FromBody] string trackingNumber = null)
        {
            var result = await _shipmentService.MarkAsShippedAsync(id, trackingNumber);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("{id}/mark-delivered")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<bool>>> MarkAsDelivered(int id)
        {
            var result = await _shipmentService.MarkAsDeliveredAsync(id);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<ShipmentStatsDto>>> GetShipmentStats()
        {
            var result = await _shipmentService.GetShipmentStatsAsync();
            return Ok(result);
        }

        [HttpGet("can-create/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> CanCreateShipment(int orderId)
        {
            var result = await _shipmentService.CanCreateShipmentAsync(orderId);
            return Ok(result);
        }
    }
}
