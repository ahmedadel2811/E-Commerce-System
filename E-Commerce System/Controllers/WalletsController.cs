using ECommerce.Core.DTOs.wallet;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ICurrentUserService _currentUserService;

        public WalletsController(IWalletService walletService, ICurrentUserService currentUserService)
        {
            _walletService = walletService;
            _currentUserService = currentUserService;
        }

        [HttpGet("MyWallet")]
        public async Task<ActionResult<Response<WalletDetailDto>>> GetMyWallet()
        {
            var result = await _walletService.GetUserWalletAsync(_currentUserService.UserId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("MyBalance")]
        public async Task<ActionResult<Response<decimal>>> GetMyBalance()
        {
            var result = await _walletService.GetBalanceAsync(_currentUserService.UserId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("MyWalletStats")]
        public async Task<ActionResult<Response<WalletStatsDto>>> GetMyWalletStats()
        {
            var result = await _walletService.GetWalletStatsAsync(_currentUserService.UserId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("Add-Balance")]
        public async Task<ActionResult<Response<WalletDto>>> AddBalance(AddBalanceDto addBalanceDto)
        {
            var result = await _walletService.AddBalanceAsync(_currentUserService.UserId, addBalanceDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Check-Balance")]
        public async Task<ActionResult<Response<bool>>> CheckBalance([FromBody] decimal amount)
        {
            var result = await _walletService.HasSufficientBalanceAsync(_currentUserService.UserId, amount);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("Transfer")]
        public async Task<ActionResult<Response<bool>>> TransferBalance([FromBody] TransferBalanceDto transferDto)
        {
            var result = await _walletService.TransferBalanceAsync(
                _currentUserService.UserId,
                transferDto.ToUserId,
                transferDto.Amount,
                transferDto.Description);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        // للمسؤولين فقط
        [HttpGet("GetUserWallet")]

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<WalletDetailDto>>> GetUserWallet(string userId)
        {
            var result = await _walletService.GetUserWalletAsync(userId);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }

   
}
