namespace ECommerce.Core.DTOs.ProductReview
{
    public class ProductReviewStatsDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int FiveStarReviews { get; set; }
        public int FourStarReviews { get; set; }
        public int ThreeStarReviews { get; set; }
        public int TwoStarReviews { get; set; }
        public int OneStarReviews { get; set; }
        public double PositiveRatingPercent { get; set; }
    }
}