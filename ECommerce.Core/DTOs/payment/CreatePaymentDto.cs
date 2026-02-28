using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.payment
{
    public class CreatePaymentDto
    {
        public int OrderId { get; set; }
        public PaymentMethod PaymentGateway { get; set; }
        public string? TransactionId { get; set; }
    }
}
