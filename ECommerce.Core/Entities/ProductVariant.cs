using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    //متغير المنتج
    public class ProductVariant : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }
        //السعر الإضافي
        public decimal? AdditionalPrice { get; set; }
        //الكمية المتاحة
        public int StockQuantity { get; set; }
    }
}
