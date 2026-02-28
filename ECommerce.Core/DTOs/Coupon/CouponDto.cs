using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs.Coupon
{
    public class CouponDto
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        public decimal? DiscountAmount { get; set; }

        [Range(0, 100)]
        public double? DiscountPercent { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int UsageLimit { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public List<int> ProductIds { get; set; } = new List<int>();

        // Calculated properties
        public bool IsExpired => ExpiryDate < System.DateTime.UtcNow;
        public bool IsValid => IsActive && !IsExpired;
    }
}
