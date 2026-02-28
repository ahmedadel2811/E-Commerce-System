using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    // الدفع
    public class Payment : BaseEntity
    {

        // حالة الدفع : معلق، مدفوع، فشل، مسترد
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; // Pending, Paid, Failed, Refunded
        public PaymentMethod PaymentGateway { get; set; } // Stripe, PayPal, COD
        //معرف المعاملة                                           
        public string TransactionId { get; set; }
        public DateTime? PaidDate { get; set; }

        // كل عملية دفع مرتبطة بطلب واحد
        public int OrderId { get; set; }
        public Order Order { get; set; }

    }
}
