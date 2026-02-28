using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.payment;
using ECommerce.Core.GenralResponse;

namespace ECommerce.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<Response<PaymentDetailDto>> GetPaymentByIdAsync(int id);
        Task<Response<PaymentDetailDto>> GetPaymentByTransactionIdAsync(string transactionId);
        Task<Response<IEnumerable<PaymentDto>>> GetUserPaymentsAsync(string userId);
        Task<Response<IEnumerable<PaymentDto>>> GetPaymentsByStatusAsync(string status);
        Task<Response<PaymentDto>> CreatePaymentAsync(CreatePaymentDto paymentDto);
        Task<Response<PaymentResultDto>> ProcessPaymentAsync(ProcessPaymentDto processPaymentDto);
        Task<Response<PaymentDto>> UpdatePaymentStatusAsync(int paymentId, string status, string transactionId = null);
        Task<Response<bool>> RefundPaymentAsync(int paymentId, string reason);
    }
}

