using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    // (تقييم المنتج)
    public class ProductReview : BaseEntity
    {

        public int Rating { get; set; } // التقييم من 1 إلى 5
        public string Comment { get; set; } // تعليق المستخدم.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // التقييم ينتمي إلى منتج واحد
        public int ProductId { get; set; }
        public Product Product { get; set; }

        //التقييم ينتمي إلى مستخدم واحد
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }

      
    }
}
