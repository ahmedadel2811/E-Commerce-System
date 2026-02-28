using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    // المحفظه
    public class Wallet : BaseEntity
    {
  
        // الرصيد الحالي في المحفظة
        public decimal Balance { get; set; } = 0;


        // تاريخ آخر تحديث
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // كل مستخدم يمتلك محفظة واحدة
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        
        // المحفظه ليها اكتر من معامله
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
