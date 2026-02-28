using ECommerce.Core.DTOs.order;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using ECommerce.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;

        public OrdersController(IOrderService orderService, ICurrentUserService currentUserService)
        {
            _orderService = orderService;
            _currentUserService = currentUserService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<Response<IEnumerable<OrderDto>>>> GetOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }

        [HttpGet("GetUserOrders")]
        public async Task<ActionResult<Response<IEnumerable<OrderDto>>>> GetUserOrders(string userId)
        {
            var result = await _orderService.GetUserOrdersAsync(userId);

            if (!result.Succeeded)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<Response<OrderDetailDto>>> GetOrder(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetByOrderNumber")]
        public async Task<ActionResult<Response<OrderDetailDto>>> GetOrderByNumber(string orderNumber)
        {
            var result = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetOrdersByStatus")]
        public async Task<ActionResult<Response<IEnumerable<OrderDto>>>> GetOrdersByStatus(string status)
        {
            var result = await _orderService.GetOrdersByStatusAsync(status);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Response<OrderDetailDto>>> CreateOrder(CreateOrderDto orderDto)
        {
            var result = await _orderService.CreateOrderAsync(orderDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetOrder), new { id = result.Data.Id }, result);
        }


        [HttpPost("from-cart")]
        public async Task<ActionResult<Response<OrderDetailDto>>> CreateOrderFromCart(CreateOrderFromCartDto orderDto)
        {
            var userId = _currentUserService.UserId;

            var result = await _orderService.CreateOrderFromCartAsync(userId, orderDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetOrder), new { id = result.Data.Id }, result);
        }

        [HttpPut("UpdateStatus")]
        public async Task<ActionResult<Response<OrderDto>>> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("UpdateOrder")]
        public async Task<ActionResult<Response<OrderDto>>> UpdateOrder(int id, UpdateOrderDto orderDto)
        {
            var result = await _orderService.UpdateOrderAsync(id, orderDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("CancelOrder")]
        public async Task<ActionResult<Response<bool>>> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetOrderStats")]
        public async Task<ActionResult<Response<OrderStatsDto>>> GetOrderStats(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _orderService.GetOrderStatsAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("GetTotalSales")]
        public async Task<ActionResult<Response<decimal>>> GetTotalSales(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _orderService.GetTotalSalesAsync(startDate, endDate);
            return Ok(result);
        }
    }
}