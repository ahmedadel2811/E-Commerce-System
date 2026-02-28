namespace ECommerce.Core.DTOs.order
{
    public class OrderStatsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}