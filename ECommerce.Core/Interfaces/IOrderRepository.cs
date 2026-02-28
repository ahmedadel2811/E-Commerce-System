using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order> GetOrderWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<Order> GetOrderByOrderNumberAsync(string orderNumber);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
