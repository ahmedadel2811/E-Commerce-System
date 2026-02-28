using ECommerce.Core.DTOs.payment;
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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ICurrentUserService _currentUserService;

        public PaymentsController(IPaymentService paymentService, ICurrentUserService currentUserService)
        {
            _paymentService = paymentService;
            _currentUserService = currentUserService;
        }

        [HttpGet("GetMyPayments")]
        public async Task<ActionResult<Response<IEnumerable<PaymentDto>>>> GetMyPayments()
        {
            var result = await _paymentService.GetUserPaymentsAsync(_currentUserService.UserId);
            return Ok(result);
        }

        [HttpGet("GetPaymentById")]
        public async Task<ActionResult<Response<PaymentDetailDto>>> GetPayment(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetPaymentByTransactionId")]
        public async Task<ActionResult<Response<PaymentDetailDto>>> GetPaymentByTransactionId(string transactionId)
        {
            var result = await _paymentService.GetPaymentByTransactionIdAsync(transactionId);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("ProcessPayment")]
        public async Task<ActionResult<Response<PaymentResultDto>>> ProcessPayment(ProcessPaymentDto processPaymentDto)
        {
            var result = await _paymentService.ProcessPaymentAsync(processPaymentDto);
            return Ok(result);
        }

        [HttpPost("RefundPayment")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<bool>>> RefundPayment(int id, [FromBody] string reason)
        {
            var result = await _paymentService.RefundPaymentAsync(id, reason);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("UpdatePaymentStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<PaymentDto>>> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusDto statusDto)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, statusDto.Status, statusDto.TransactionId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

       
       
    }

   
}