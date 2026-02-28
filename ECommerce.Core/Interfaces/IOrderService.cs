using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.order;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Response<OrderDetailDto>> GetOrderByIdAsync(int id);
        Task<Response<OrderDetailDto>> GetOrderByOrderNumberAsync(string orderNumber);
        Task<Response<IEnumerable<OrderDto>>> GetUserOrdersAsync(string userId);
        Task<Response<IEnumerable<OrderDto>>> GetAllOrdersAsync();
        Task<Response<IEnumerable<OrderDto>>> GetOrdersByStatusAsync(string status);
        Task<Response<OrderDetailDto>> CreateOrderAsync(CreateOrderDto orderDto);
        Task<Response<OrderDetailDto>> CreateOrderFromCartAsync(string userId, CreateOrderFromCartDto orderDto);

        Task<Response<OrderDto>> UpdateOrderStatusAsync(int id, string status);
        Task<Response<OrderDto>> UpdateOrderAsync(int id, UpdateOrderDto orderDto);
        Task<Response<bool>> CancelOrderAsync(int id);
        Task<Response<OrderStatsDto>> GetOrderStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Response<decimal>> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}