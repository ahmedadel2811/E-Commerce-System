using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs.product
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double? DiscountPercent { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public int? VendorId { get; set; }
        public int? CategoryId { get; set; }
        public string ImageUrl { get; set; }
    }
}
