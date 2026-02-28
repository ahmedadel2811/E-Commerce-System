using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.payment
{
    public class ProcessPaymentDto
    {
        public int OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? NameOnCard { get; set; }
    }
}
