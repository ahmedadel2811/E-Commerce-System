namespace ECommerce.Core.DTOs.wallet
{
    public class WalletDetailDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<WalletTransactionDto> Transactions { get; set; } = new List<WalletTransactionDto>();
    }
}
