namespace ECommerce.Core.DTOs.wallet
{
    public class WalletTransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // Credit / Debit
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
