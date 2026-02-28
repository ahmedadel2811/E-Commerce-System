namespace ECommerce.Core.DTOs.payment
{
    public class PaymentStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public int SuccessfulPayments { get; set; }
        public int PendingPayments { get; set; }
        public int FailedPayments { get; set; }
        public decimal AverageTransactionValue { get; set; }
    }
}
