using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{


    // معاملة المحفظة
    public class WalletTransaction : BaseEntity
    {
        public decimal Amount { get; set; } // المبلغ الذي تم إضافته أو خصمه
        public string Type { get; set; } // Credit / Debit نوع المعاملة إضافة أو خصم
        public string Description { get; set; } // وصف المعاملة
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // المعاملة تتعلق بمحفظة واحدة
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
    }
}
