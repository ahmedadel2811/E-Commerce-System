using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Enums;

namespace ECommerce.Core.DTOs.payment
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusDisplay { get; set; }
        public PaymentMethod PaymentGateway { get; set; }
        public string PaymentGatewayDisplay { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
