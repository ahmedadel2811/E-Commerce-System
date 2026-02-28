using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Service.Repositories
{
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(AppDbContext context) : base(context) { }

        public async Task<Wallet> GetUserWalletAsync(string userId)
        {
            return await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wallet> GetWalletWithTransactionsAsync(string userId )
        {
            return await _context.Wallets
                .Include(w => w.User)
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task UpdateWalletBalanceAsync(int walletId, decimal amount)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet != null)
            {
                wallet.Balance += amount;
                wallet.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetWalletBalanceAsync(string userId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.Balance ?? 0;
        }

        public async Task<bool> HasSufficientBalanceAsync(string userId, decimal amount)
        {
            var balance = await GetWalletBalanceAsync(userId);
            return balance >= amount;
        }
    }
}