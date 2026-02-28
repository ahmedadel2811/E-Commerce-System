using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.wallet;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IGenericRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public WalletService(
            IWalletRepository walletRepository,
            IGenericRepository<WalletTransaction> walletTransactionRepository,
            IGenericRepository<ApplicationUser> userRepository,
            ICurrentUserService currentUserService)
        {
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Response<WalletDetailDto>> GetUserWalletAsync(string userId)
        {
            try
            {
                var wallet = await _walletRepository.GetWalletWithTransactionsAsync(userId);
                if (wallet == null)
                {
                    // لو مفيش محفظه نعمل واحدة جديدة
                    wallet = await CreateWalletForUserAsync(userId);
                }

                var walletDto = MapToWalletDetailDto(wallet);
                return Response<WalletDetailDto>.Success(walletDto, "Wallet retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<WalletDetailDto>.Fail($"Error retrieving wallet: {ex.Message}");
            }
        }

        public async Task<Response<WalletDto>> GetWalletByUserIdAsync(string userId)
        {
            try
            {
                var wallet = await _walletRepository.GetUserWalletAsync(userId);
                if (wallet == null)
                {
                    wallet = await CreateWalletForUserAsync(userId);
                }

                var walletDto = MapToWalletDto(wallet);
                return Response<WalletDto>.Success(walletDto, "Wallet retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<WalletDto>.Fail($"Error retrieving wallet: {ex.Message}");
            }
        }

        public async Task<Response<WalletDto>> AddBalanceAsync(string userId, AddBalanceDto addBalanceDto)
        {
            try
            {
                if (addBalanceDto.Amount <= 0)
                    return Response<WalletDto>.Fail("Amount must be greater than zero");

                var wallet = await _walletRepository.GetUserWalletAsync(userId);
                if (wallet == null)
                {
                    wallet = await CreateWalletForUserAsync(userId);
                }

                // نضيف الرصيد
                await _walletRepository.UpdateWalletBalanceAsync(wallet.Id, addBalanceDto.Amount);

                // نعمل transaction record
                var transaction = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = addBalanceDto.Amount,
                    Type = "Credit",
                    Description = addBalanceDto.Description ?? "Balance added",
                    CreatedAt = DateTime.UtcNow
                };
                await _walletTransactionRepository.AddAsync(transaction);

                // نجيب المحفظه المحدثة
                var updatedWallet = await _walletRepository.GetUserWalletAsync(userId);
                var walletDto = MapToWalletDto(updatedWallet);
                return Response<WalletDto>.Success(walletDto, "Balance added successfully");
            }
            catch (Exception ex)
            {
                return Response<WalletDto>.Fail($"Error adding balance: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeductBalanceAsync(string userId, decimal amount, string description)
        {
            try
            {
                if (amount <= 0)
                    return Response<bool>.Fail("Amount must be greater than zero");

                var hasBalance = await _walletRepository.HasSufficientBalanceAsync(userId, amount);
                if (!hasBalance)
                    return Response<bool>.Fail("Insufficient balance");

                var wallet = await _walletRepository.GetUserWalletAsync(userId);
                if (wallet == null)
                    return Response<bool>.Fail("Wallet not found");

                // نخصم الرصيد
                await _walletRepository.UpdateWalletBalanceAsync(wallet.Id, -amount);

                // نعمل
                // transaction record
                var transaction = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = amount,
                    Type = "Debit",
                    Description = description ?? "Balance deducted",
                    CreatedAt = DateTime.UtcNow
                };
                await _walletTransactionRepository.AddAsync(transaction);

                return Response<bool>.Success(true, "Balance deducted successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error deducting balance: {ex.Message}");
            }
        }

        public async Task<Response<bool>> TransferBalanceAsync(string fromUserId, string toUserId, decimal amount, string description)
        {
            try
            {
                if (amount <= 0)
                    return Response<bool>.Fail("Amount must be greater than zero");

                // نتأكد إن الـ
                // from user
                // عنده رصيد كافي
                var hasBalance = await _walletRepository.HasSufficientBalanceAsync(fromUserId, amount);
                if (!hasBalance)
                    return Response<bool>.Fail("Insufficient balance for transfer");

                var fromWallet = await _walletRepository.GetUserWalletAsync(fromUserId);
                var toWallet = await _walletRepository.GetUserWalletAsync(toUserId);

                if (fromWallet == null)
                    return Response<bool>.Fail("Sender wallet not found");

                if (toWallet == null)
                {
                    // نعمله wallet ممكن فى بعدين لو مش موجود له
                    var toUser = await _userRepository.GetByIdAsync(toUserId);
                    if (toUser == null)
                        return Response<bool>.Fail("Recipient user not found");

                    toWallet = new Wallet
                    {
                        UserId = toUserId,
                        Balance = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                    toWallet = await _walletRepository.AddAsync(toWallet);
                }

                // ننفذ التحويل
                await _walletRepository.UpdateWalletBalanceAsync(fromWallet.Id, -amount);
                await _walletRepository.UpdateWalletBalanceAsync(toWallet.Id, amount);

                // نعمل
                // transaction records
                var debitTransaction = new WalletTransaction
                {
                    WalletId = fromWallet.Id,
                    Amount = amount,
                    Type = "Debit",
                    Description = $"Transfer to {toWallet.User.FullName}: {description}",
                    CreatedAt = DateTime.UtcNow
                };

                var creditTransaction = new WalletTransaction
                {
                    WalletId = toWallet.Id,
                    Amount = amount,
                    Type = "Credit",
                    Description = $"Transfer from {fromWallet.User.FullName}: {description}",
                    CreatedAt = DateTime.UtcNow
                };

                await _walletTransactionRepository.AddAsync(debitTransaction);
                await _walletTransactionRepository.AddAsync(creditTransaction);

                return Response<bool>.Success(true, "Balance transferred successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error transferring balance: {ex.Message}");
            }
        }

        public async Task<Response<WalletStatsDto>> GetWalletStatsAsync(string userId)
        {
            try
            {
                var wallet = await _walletRepository.GetWalletWithTransactionsAsync(userId);
                if (wallet == null)
                    return Response<WalletStatsDto>.Fail("Wallet not found");

                var transactions = wallet.Transactions;
                var stats = new WalletStatsDto
                {
                    CurrentBalance = wallet.Balance,
                    TotalCredits = transactions.Where(t => t.Type == "Credit").Sum(t => t.Amount),
                    TotalDebits = transactions.Where(t => t.Type == "Debit").Sum(t => t.Amount),
                    TransactionsCount = transactions.Count
                };

                return Response<WalletStatsDto>.Success(stats, "Wallet stats retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<WalletStatsDto>.Fail($"Error retrieving wallet stats: {ex.Message}");
            }
        }

        public async Task<Response<decimal>> GetBalanceAsync(string userId)
        {
            try
            {
                var balance = await _walletRepository.GetWalletBalanceAsync(userId);
                return Response<decimal>.Success(balance, "Balance retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Fail($"Error retrieving balance: {ex.Message}");
            }
        }

        public async Task<Response<bool>> HasSufficientBalanceAsync(string userId, decimal amount)
        {
            try
            {
                var hasBalance = await _walletRepository.HasSufficientBalanceAsync(userId, amount);
                return Response<bool>.Success(hasBalance, "Balance check completed");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error checking balance: {ex.Message}");
            }
        }

        private async Task<Wallet> CreateWalletForUserAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var wallet = new Wallet
            {
                UserId = userId,
                Balance = 0,
                LastUpdated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return await _walletRepository.AddAsync(wallet);
        }

        // Manual Mapping Methods
        private WalletDto MapToWalletDto(Wallet wallet)
        {
            return new WalletDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                UserName = wallet.User?.FullName,
                Balance = wallet.Balance,
                LastUpdated = wallet.LastUpdated,
                CreatedAt = wallet.CreatedAt
            };
        }

        private WalletDetailDto MapToWalletDetailDto(Wallet wallet)
        {
            return new WalletDetailDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                UserName = wallet.User?.FullName,
                UserEmail = wallet.User?.Email,
                Balance = wallet.Balance,
                LastUpdated = wallet.LastUpdated,
                Transactions = wallet.Transactions?.Select(t => new WalletTransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Type = t.Type,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                }).ToList() ?? new List<WalletTransactionDto>()
            };
        }
    }
}