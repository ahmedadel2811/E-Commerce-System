using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs.product
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string VendorShopName { get; set; }
        //public List<ProductImageDto> Images { get; set; } = new();
        //public List<ProductVariantDto> Variants { get; set; } = new();
        //public List<ProductReviewDto> Reviews { get; set; } = new();
    }
}
