namespace ECommerce.Core.DTOs.Coupon
{
    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public decimal DiscountAmount { get; set; }
        public CouponDto Coupon { get; set; }
    }
}
