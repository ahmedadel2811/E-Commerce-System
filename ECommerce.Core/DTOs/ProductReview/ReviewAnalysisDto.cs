namespace ECommerce.Core.DTOs.ProductReview
{
    public class ReviewAnalysisDto
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int VerifiedPurchases { get; set; }
        public int PendingReviews { get; set; }
        public Dictionary<string, int> ReviewsByProduct { get; set; } = new Dictionary<string, int>();
    }
}