using System.Data;
using ECommerce.Core.DTOs.deliveryagent;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryAgentsController : ControllerBase
    {
        private readonly IDeliveryAgentService _deliveryAgentService;
        private readonly ICurrentUserService _currentUserService;

        public DeliveryAgentsController(IDeliveryAgentService deliveryAgentService, ICurrentUserService currentUserService)
        {
            _deliveryAgentService = deliveryAgentService;
            _currentUserService = currentUserService;
        }

        [HttpGet("GetDeliveryAgents")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<IEnumerable<DeliveryAgentDto>>>> GetDeliveryAgents()
        {
            var result = await _deliveryAgentService.GetAllDeliveryAgentsAsync();
            return Ok(result);
        }

        [HttpGet("GetAvailableDeliveryAgents")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<IEnumerable<DeliveryAgentDto>>>> GetAvailableDeliveryAgents()
        {
            var result = await _deliveryAgentService.GetAvailableDeliveryAgentsAsync();
            return Ok(result);
        }

        [HttpGet("GetDeliveryAgentsByStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<IEnumerable<DeliveryAgentDto>>>> GetDeliveryAgentsByStatus(string status)
        {
            var result = await _deliveryAgentService.GetDeliveryAgentsByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<DeliveryAgentDetailDto>>> GetDeliveryAgent(int id)
        {
            // Delivery agents can only view their own profile unless they're admin
            if (_currentUserService.Roles.Contains("DeliveryAgent") && !_currentUserService.Roles.Contains("Admin"))
            {
                // لكن مش مسؤؤل  DeliveryAgent  لو المستخدم الحالي عنده الدور
                //يبقى هو مندوب عادي مش مسؤول إداري
                // هنا بنجيب بيانات المندوب اللي داخل دلوقتي  
                var currentAgent = await _deliveryAgentService.GetCurrentDeliveryAgentAsync(_currentUserService.UserId);
                if (!currentAgent.Succeeded || currentAgent.Data.Id != id)
                    return Forbid();
            }

            var result = await _deliveryAgentService.GetDeliveryAgentByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetMyProfile")]
        [Authorize(Roles = "DeliveryAgent")]
        public async Task<ActionResult<Response<DeliveryAgentDto>>> GetMyProfile()
        {
            var result = await _deliveryAgentService.GetCurrentDeliveryAgentAsync(_currentUserService.UserId);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetMyDetailedProfile")]
        [Authorize(Roles = "DeliveryAgent")]
        public async Task<ActionResult<Response<DeliveryAgentDetailDto>>> GetMyDetailedProfile()
        {
            var result = await _deliveryAgentService.GetDeliveryAgentByUserIdAsync(_currentUserService.UserId);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<DeliveryAgentDto>>> CreateDeliveryAgent(CreateDeliveryAgentDto deliveryAgentDto)
        {
            var result = await _deliveryAgentService.CreateDeliveryAgentAsync(deliveryAgentDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetDeliveryAgent), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<DeliveryAgentDto>>> UpdateDeliveryAgent(int id, UpdateDeliveryAgentDto deliveryAgentDto)
        {
            var result = await _deliveryAgentService.UpdateDeliveryAgentAsync(id, deliveryAgentDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("DeleteDeliveryAgent")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> DeleteDeliveryAgent(int id)
        {
            var result = await _deliveryAgentService.DeleteDeliveryAgentAsync(id);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<DeliveryAgentDto>>> UpdateDeliveryAgentStatus(int id, [FromBody] string status)
        {
            // Delivery agents can only update their own status
            if (_currentUserService.Roles.Contains("DeliveryAgent") && !_currentUserService.Roles.Contains("Admin"))
            {
                var currentAgent = await _deliveryAgentService.GetCurrentDeliveryAgentAsync(_currentUserService.UserId);
                if (!currentAgent.Succeeded || currentAgent.Data.Id != id)
                    return Forbid();
            }

            var result = await _deliveryAgentService.UpdateDeliveryAgentStatusAsync(id, status);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("AssignShipmentToAgent")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> AssignShipmentToAgent(AssignShipmentToAgentDto assignDto)
        {
            var result = await _deliveryAgentService.AssignShipmentToAgentAsync(assignDto.ShipmentId, assignDto.DeliveryAgentId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<DeliveryAgentStatsDto>>> GetDeliveryAgentStats()
        {
            var result = await _deliveryAgentService.GetDeliveryAgentStatsAsync();
            return Ok(result);
        }

        [HttpGet("GetActiveShipmentsCount")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<int>>> GetActiveShipmentsCount(int id)
        {
            // Delivery agents can only check their own active shipments count
            if (_currentUserService.Roles.Contains("DeliveryAgent") && !_currentUserService.Roles.Contains("Admin"))
            {
                var currentAgent = await _deliveryAgentService.GetCurrentDeliveryAgentAsync(_currentUserService.UserId);
                if (!currentAgent.Succeeded || currentAgent.Data.Id != id)
                    return Forbid();
            }

            var result = await _deliveryAgentService.GetActiveShipmentsCountAsync(id);
            return Ok(result);
        }

        [HttpPost("MarkAsAvailable")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<DeliveryAgentDto>>> MarkAsAvailable(int id)
        {
            // Delivery agents can only mark themselves as available
            if (_currentUserService.Roles.Contains("DeliveryAgent") && !_currentUserService.Roles.Contains("Admin"))
            {
                var currentAgent = await _deliveryAgentService.GetCurrentDeliveryAgentAsync(_currentUserService.UserId);
                if (!currentAgent.Succeeded || currentAgent.Data.Id != id)
                    return Forbid();
            }

            var result = await _deliveryAgentService.UpdateDeliveryAgentStatusAsync(id, "Available");
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("MarkAsBusy")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<DeliveryAgentDto>>> MarkAsBusy(int id)
        {
            // Delivery agents can only mark themselves as busy
            if (_currentUserService.Roles.Contains("DeliveryAgent") && !_currentUserService.Roles.Contains("Admin"))
            {
                var currentAgent = await _deliveryAgentService.GetCurrentDeliveryAgentAsync(_currentUserService.UserId);
                if (!currentAgent.Succeeded || currentAgent.Data.Id != id)
                    return Forbid();
            }

            var result = await _deliveryAgentService.UpdateDeliveryAgentStatusAsync(id, "Busy");
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("MarkAsOffline")]
        [Authorize(Roles = "Admin,DeliveryAgent")]
        public async Task<ActionResult<Response<DeliveryAgentDto>>> MarkAsOffline(int id)
        {
            // Delivery agents can only mark themselves as offline
            if (_currentUserService.Roles.Contains("DeliveryAgent") && !_currentUserService.Roles.Contains("Admin"))
            {
                var currentAgent = await _deliveryAgentService.GetCurrentDeliveryAgentAsync(_currentUserService.UserId);
                if (!currentAgent.Succeeded || currentAgent.Data.Id != id)
                    return Forbid();
            }

            var result = await _deliveryAgentService.UpdateDeliveryAgentStatusAsync(id, "Offline");
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}