using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ImageUrl { get; set; } // رابط الصورة
        public bool IsMain { get; set; } = false; //  إذا كانت هذه الصورة هي الصورة الرئيسية للمنتج
    }
}
