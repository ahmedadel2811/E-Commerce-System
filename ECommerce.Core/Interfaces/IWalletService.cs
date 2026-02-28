using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.wallet;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IWalletService
    {
        Task<Response<WalletDetailDto>> GetUserWalletAsync(string userId);
        Task<Response<WalletDto>> GetWalletByUserIdAsync(string userId);
        Task<Response<WalletDto>> AddBalanceAsync(string userId, AddBalanceDto addBalanceDto);
        Task<Response<bool>> DeductBalanceAsync(string userId, decimal amount, string description); // خصم الرصيد
        Task<Response<bool>> TransferBalanceAsync(string fromUserId, string toUserId, decimal amount, string description);
        Task<Response<WalletStatsDto>> GetWalletStatsAsync(string userId);
        Task<Response<decimal>> GetBalanceAsync(string userId);
        Task<Response<bool>> HasSufficientBalanceAsync(string userId, decimal amount);
    }
}