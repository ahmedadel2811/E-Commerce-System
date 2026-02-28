using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.order;
using ECommerce.Core.DTOs.payment;
using ECommerce.Core.DTOs.wallet;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWalletService _walletService;
        private readonly IGenericRepository<AuditLog> _auditLogRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IWalletService walletService,
            IGenericRepository<AuditLog> auditLogRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _walletService = walletService;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Response<PaymentDetailDto>> GetPaymentByIdAsync(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentWithOrderAsync(id);
                if (payment == null)
                    return Response<PaymentDetailDto>.Fail("Payment not found");

                var paymentDto = MapToPaymentDetailDto(payment);
                return Response<PaymentDetailDto>.Success(paymentDto, "Payment retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<PaymentDetailDto>.Fail($"Error retrieving payment: {ex.Message}");
            }
        }

        public async Task<Response<PaymentDetailDto>> GetPaymentByTransactionIdAsync(string transactionId)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByTransactionIdAsync(transactionId);
                if (payment == null)
                    return Response<PaymentDetailDto>.Fail("Payment not found");

                var paymentDto = MapToPaymentDetailDto(payment);
                return Response<PaymentDetailDto>.Success(paymentDto, "Payment retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<PaymentDetailDto>.Fail($"Error retrieving payment: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<PaymentDto>>> GetUserPaymentsAsync(string userId)
        {
            try
            {
                var payments = await _paymentRepository.GetUserPaymentsAsync(userId);
                var paymentDtos = payments.Select(MapToPaymentDto).ToList();
                return Response<IEnumerable<PaymentDto>>.Success(paymentDtos, "User payments retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<PaymentDto>>.Fail($"Error retrieving user payments: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<PaymentDto>>> GetPaymentsByStatusAsync(string status)
        {
            try
            {
                var payments = await _paymentRepository.GetPaymentsByStatusAsync(status);
                var paymentDtos = payments.Select(MapToPaymentDto).ToList();
                return Response<IEnumerable<PaymentDto>>.Success(paymentDtos, $"{status} payments retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<PaymentDto>>.Fail($"Error retrieving {status} payments: {ex.Message}");
            }
        }

        public async Task<Response<PaymentDto>> CreatePaymentAsync(CreatePaymentDto paymentDto)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(paymentDto.OrderId);
                if (order == null)
                    return Response<PaymentDto>.Fail("Order not found");

                // نتأكد إن مفيش
                // payment
                // موجودة للطلب
                var existingPayment = await _paymentRepository.FindAsync(p => p.OrderId == paymentDto.OrderId);
                if (existingPayment.Any())
                    return Response<PaymentDto>.Fail("Payment already exists for this order");

                var payment = new Payment
                {
                    OrderId = paymentDto.OrderId,
                    PaymentStatus = Core.Enums.PaymentStatus.Pending,
                    PaymentGateway = paymentDto.PaymentGateway,
                    TransactionId = paymentDto.TransactionId,
                    CreatedAt = DateTime.UtcNow
                };

                var createdPayment = await _paymentRepository.AddAsync(payment);

                // نعمل audit log
                await CreateAuditLog("Payment", "Create", $"Payment created for order #{order.OrderNumber}", order.UserId);

                var resultDto = MapToPaymentDto(createdPayment);
                return Response<PaymentDto>.Success(resultDto, "Payment created successfully");
            }
            catch (Exception ex)
            {
                return Response<PaymentDto>.Fail($"Error creating payment: {ex.Message}");
            }
        }

        public async Task<Response<PaymentResultDto>> ProcessPaymentAsync(ProcessPaymentDto processPaymentDto)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithDetailsAsync(processPaymentDto.OrderId);
                if (order == null)
                    return Response<PaymentResultDto>.Fail("Order not found");

                // نتأكد
                // إن الطلب لسه
                // pending
                if (order.Status != Core.Enums.OrderStatus.Pending)    
                    return Response<PaymentResultDto>.Fail("Order cannot be paid");

                PaymentResultDto paymentResult;

                // معالجة الدفع بناءً على طريقة الدفع
                switch (processPaymentDto.PaymentMethod)
                {
                    case PaymentMethod.Wallet:
                        paymentResult = await ProcessWalletPayment(order);
                        break;
                    case PaymentMethod.CreditCard:
                        paymentResult = await ProcessCreditCardPayment(order, processPaymentDto);
                        break;
                    case PaymentMethod.PayPal:
                        paymentResult = await ProcessPayPalPayment(order);
                        break;
                    case PaymentMethod.COD:
                        paymentResult = await ProcessCODPayment(order);
                        break;
                    default:
                        return Response<PaymentResultDto>.Fail("Unsupported payment method");
                }

                if (paymentResult.Success)
                {
                    // نحدث حالة الطلب
                    await _orderRepository.UpdateOrderStatusAsync(order.Id, "Confirmed");

                    // نعمل payment record
                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        PaymentStatus = Core.Enums.PaymentStatus.Paid,
                        PaymentGateway = processPaymentDto.PaymentMethod,
                        TransactionId = paymentResult.TransactionId,
                        PaidDate = paymentResult.PaidDate,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _paymentRepository.AddAsync(payment);

                    // نعمل audit log
                    await CreateAuditLog("Payment", "Process",
                        $"Payment processed successfully for order #{order.OrderNumber}. Transaction: {paymentResult.TransactionId}",
                        order.UserId);
                }
                else
                {
                    // نعمل payment record failed
                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        PaymentStatus = Core.Enums.PaymentStatus.Failed,
                        PaymentGateway = processPaymentDto.PaymentMethod,
                        TransactionId = paymentResult.TransactionId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _paymentRepository.AddAsync(payment);

                    await CreateAuditLog("Payment", "Failed",
                        $"Payment failed for order #{order.OrderNumber}. Reason: {paymentResult.Message}",
                        order.UserId);
                }

                return Response<PaymentResultDto>.Success(paymentResult,
                    paymentResult.Success ? "Payment processed successfully" : "Payment failed");
            }
            catch (Exception ex)
            {
                return Response<PaymentResultDto>.Fail($"Error processing payment: {ex.Message}");
            }
        }

        public async Task<Response<PaymentDto>> UpdatePaymentStatusAsync(int paymentId, string status, string transactionId = null)
        {
            try
            {
                await _paymentRepository.UpdatePaymentStatusAsync(paymentId, status, transactionId);
                var updatedPayment = await _paymentRepository.GetByIdAsync(paymentId);
                var paymentDto = MapToPaymentDto(updatedPayment);
                return Response<PaymentDto>.Success(paymentDto, "Payment status updated successfully");
            }
            catch (Exception ex)
            {
                return Response<PaymentDto>.Fail($"Error updating payment status: {ex.Message}");
            }
        }

        public async Task<Response<bool>> RefundPaymentAsync(int paymentId, string reason)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentWithOrderAsync(paymentId);
                if (payment == null)
                    return Response<bool>.Fail("Payment not found");

                if (payment.PaymentStatus != Core.Enums.PaymentStatus.Paid)
                    return Response<bool>.Fail("Only paid payments can be refunded");

                // منطق الاسترجاع بناءً على طريقة الدفع
                if (payment.PaymentGateway == PaymentMethod.Wallet)
                {
                    // نرجع المبلغ للمحفظة
                    var refundResult = await _walletService.AddBalanceAsync(
                        payment.Order.UserId,
                        new AddBalanceDto
                        {
                            Amount = payment.Order.FinalAmount,
                            Description = $"Refund for order #{payment.Order.OrderNumber}. Reason: {reason}"
                        });

                    if (!refundResult.Succeeded)
                        return Response<bool>.Fail($"Refund failed: {refundResult.Message}");
                }

                // نحدث حالة الدفع
                await _paymentRepository.UpdatePaymentStatusAsync(paymentId, "Refunded");

                // نحدث حالة الطلب
                await _orderRepository.UpdateOrderStatusAsync(payment.OrderId, "Refunded");

                await CreateAuditLog("Payment", "Refund",
                    $"Payment refunded for order #{payment.Order.OrderNumber}. Reason: {reason}",
                    payment.Order.UserId);

                return Response<bool>.Success(true, "Payment refunded successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error refunding payment: {ex.Message}");
            }
        }

        

        

        // Methods
        // لمعالجة أنواع الدفع المختلفة
        private async Task<PaymentResultDto> ProcessWalletPayment(Order order)
        {
            try
            {
                //  نتأكد إن الرصيد كافي
                var hasBalance = await _walletService.HasSufficientBalanceAsync(order.UserId, order.FinalAmount);
                if (!hasBalance.Data)      
                    return new PaymentResultDto { Success = false, Message = "Insufficient wallet balance" };

                // نخصم من المحفظة
                var deductResult = await _walletService.DeductBalanceAsync(
                    order.UserId,
                    order.FinalAmount,
                    $"Payment for order #{order.OrderNumber}");

                if (!deductResult.Succeeded)
                    return new PaymentResultDto { Success = false, Message = deductResult.Message };

                //  نرجع نتيجة ناجحة
                return new PaymentResultDto
                {
                    Success = true,
                    TransactionId = GenerateTransactionId(),
                    Message = "Wallet payment successful",
                    Amount = order.FinalAmount,
                    PaidDate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = $"Wallet payment error: {ex.Message}"
                };
            }
        }
        private async Task<PaymentResultDto> ProcessCreditCardPayment(Order order, ProcessPaymentDto paymentDto)
        {
            // محاكاة معالجة البطاقة الائتمانية
            //  gateway زي Stripe أو PayPal
            await Task.Delay(1000); 

            var success = SimulateCreditCardPayment(paymentDto);

            if (success)
            {
                return new PaymentResultDto
                {
                    Success = true,
                    TransactionId = GenerateTransactionId(),
                    Message = "Credit card payment successful",
                    Amount = order.FinalAmount,
                    PaidDate = DateTime.UtcNow
                };
            }
            else
            {
                return new PaymentResultDto
                {
                    Success = false,
                    TransactionId = GenerateTransactionId(),
                    Message = "Credit card payment failed - please check your card details"
                };
            }
        }

        private async Task<PaymentResultDto> ProcessPayPalPayment(Order order)
        {
            // محاكاة معالجة PayPal
            await Task.Delay(1500);

            return new PaymentResultDto
            {
                Success = true,
                TransactionId = GenerateTransactionId(),
                Message = "PayPal payment successful",
                Amount = order.FinalAmount,
                PaidDate = DateTime.UtcNow
            };
        }

        private async Task<PaymentResultDto> ProcessCODPayment(Order order)
        {
            // الدفع عند الاستلام  ما بنحتاج معالجة 
            await Task.Delay(500);

            return new PaymentResultDto
            {
                Success = true,
                TransactionId = GenerateTransactionId(),
                Message = "Cash on delivery order confirmed",
                Amount = order.FinalAmount,
                PaidDate = null // مش بيدفع دلوقتي
            };
        }

        private bool SimulateCreditCardPayment(ProcessPaymentDto paymentDto)
        {
            // محاكاة بسيطة  gateway
            return !string.IsNullOrEmpty(paymentDto.CardNumber) &&
                   !string.IsNullOrEmpty(paymentDto.ExpiryDate) &&
                   !string.IsNullOrEmpty(paymentDto.CVV) &&
                   paymentDto.CardNumber.Length == 16;
        }

        private string GenerateTransactionId()
        {
            return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private async Task CreateAuditLog(string entityName, string actionType, string description, string userId)
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                ActionType = actionType,
                UserId = userId,
                NewValues = description,
                DateTime = DateTime.UtcNow
            };
            await _auditLogRepository.AddAsync(auditLog);
        }

        // Manual Mapping Methods
        private PaymentDto MapToPaymentDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                OrderNumber = payment.Order?.OrderNumber,
                PaymentStatus = payment.PaymentStatus,
                PaymentStatusDisplay = payment.PaymentStatus.ToString(),
                PaymentGateway = payment.PaymentGateway,
                PaymentGatewayDisplay = payment.PaymentGateway.ToString(),
                TransactionId = payment.TransactionId,
                Amount = payment.Order?.FinalAmount ?? 0,
                PaidDate = payment.PaidDate,
                CreatedAt = payment.CreatedAt
            };
        }

        private PaymentDetailDto MapToPaymentDetailDto(Payment payment)
        {
            return new PaymentDetailDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                OrderNumber = payment.Order?.OrderNumber,
                UserId = payment.Order?.UserId,
                UserName = payment.Order?.User?.FullName,
                UserEmail = payment.Order?.User?.Email,
                PaymentStatus = payment.PaymentStatus,
                PaymentStatusDisplay = payment.PaymentStatus.ToString(),
                PaymentGateway = payment.PaymentGateway,
                PaymentGatewayDisplay = payment.PaymentGateway.ToString(),
                TransactionId = payment.TransactionId,
                Amount = payment.Order?.FinalAmount ?? 0,
                PaidDate = payment.PaidDate,
                CreatedAt = payment.CreatedAt,
                Order = payment.Order != null ? new OrderDto
                {
                    Id = payment.Order.Id,
                    OrderNumber = payment.Order.OrderNumber,
                    TotalAmount = payment.Order.TotalAmount,
                    FinalAmount = payment.Order.FinalAmount,
                    Status = payment.Order.Status
                } : null
            };
        }
    }
}