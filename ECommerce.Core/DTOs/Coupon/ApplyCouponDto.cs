using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.DTOs.Coupon
{
    public class ApplyCouponDto
    {
        [Required]
        public string Code { get; set; }

        public List<int> ProductIds { get; set; } = new List<int>();

        public decimal OrderTotal { get; set; }
    }
}
