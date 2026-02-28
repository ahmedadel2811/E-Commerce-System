namespace ECommerce.Core.DTOs.wallet
{
    public class WalletStatsDto
    {
        public decimal CurrentBalance { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public int TransactionsCount { get; set; }
    }
}
