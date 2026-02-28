using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.DTOs.Coupon
{
    public class CreateCouponDto
    {
        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        public decimal? DiscountAmount { get; set; }

        [Range(0, 100)]
        public double? DiscountPercent { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int UsageLimit { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
