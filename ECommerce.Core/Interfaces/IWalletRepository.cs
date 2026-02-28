using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IWalletRepository : IGenericRepository<Wallet>
    {
        Task<Wallet> GetUserWalletAsync(string userId);
        Task<Wallet> GetWalletWithTransactionsAsync(string userId);
        Task UpdateWalletBalanceAsync(int walletId, decimal amount);
        Task<decimal> GetWalletBalanceAsync(string userId);
        Task<bool> HasSufficientBalanceAsync(string userId, decimal amount);
    }
}
