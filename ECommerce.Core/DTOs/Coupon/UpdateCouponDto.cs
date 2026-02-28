using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.DTOs.Coupon
{
    public class UpdateCouponDto
    {
        public string Code { get; set; } 

        public decimal? DiscountAmount { get; set; }

        [Range(0, 100)]
        public double? DiscountPercent { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int? UsageLimit { get; set; }

        public bool? IsActive { get; set; }

        public List<int> ProductIds { get; set; }
    }
}
