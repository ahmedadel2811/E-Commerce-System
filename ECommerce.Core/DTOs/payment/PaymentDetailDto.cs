using ECommerce.Core.DTOs.order;
using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.payment
{
    public class PaymentDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusDisplay { get; set; }
        public PaymentMethod PaymentGateway { get; set; }
        public string PaymentGatewayDisplay { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderDto Order { get; set; }
    }
}
