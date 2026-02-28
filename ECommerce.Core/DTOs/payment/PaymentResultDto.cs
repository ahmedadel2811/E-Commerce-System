namespace ECommerce.Core.DTOs.payment
{
    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
