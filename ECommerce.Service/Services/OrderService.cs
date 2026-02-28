using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Core.DTOs.order;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;
using ECommerce.Service.Repositories;

namespace ECommerce.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICartRepository _cartRepository;
        private readonly IVendorRepository _vendorRepository;


        private readonly IWalletService _walletService;

        public OrderService(
            IOrderRepository orderRepository,
            IGenericRepository<OrderItem> orderItemRepository,
            IProductRepository productRepository,
            ICurrentUserService currentUserService
,
            ICartRepository cartRepository,
            IWalletService walletService,
            IVendorRepository vendorRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _cartRepository = cartRepository;
            _walletService = walletService;
            _vendorRepository = vendorRepository;
        }

        public async Task<Response<OrderDetailDto>> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithDetailsAsync(id);
                if (order == null)
                    return Response<OrderDetailDto>.Fail("Order not found");

                var orderDto = MapToOrderDetailDto(order);
                return Response<OrderDetailDto>.Success(orderDto, "Order retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDetailDto>.Fail($"Error retrieving order: {ex.Message}");
            }
        }

        public async Task<Response<OrderDetailDto>> GetOrderByOrderNumberAsync(string orderNumber)
        {
            try
            {
                var order = await _orderRepository.GetOrderByOrderNumberAsync(orderNumber);
                if (order == null)
                    return Response<OrderDetailDto>.Fail("Order not found");

                var orderDto = MapToOrderDetailDto(order);
                return Response<OrderDetailDto>.Success(orderDto, "Order retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDetailDto>.Fail($"Error retrieving order: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<OrderDto>>> GetUserOrdersAsync(string userId)
        {
            try
            {
                var orders = await _orderRepository.GetUserOrdersAsync(userId);

                if (orders?.Any() != true)
                {
                    return Response<IEnumerable<OrderDto>>.Fail($"no order for  user ");

                }
                var orderDtos = orders.Select(MapToOrderDto).ToList();
                return Response<IEnumerable<OrderDto>>.Success(orderDtos, "User orders retrieved successfully");


            }
            catch (Exception ex)
            {
                return Response<IEnumerable<OrderDto>>.Fail($"Error retrieving user orders: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<OrderDto>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                var orderDtos = orders.Select(MapToOrderDto).ToList();
                return Response<IEnumerable<OrderDto>>.Success(orderDtos, "Orders retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<OrderDto>>.Fail($"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<OrderDto>>> GetOrdersByStatusAsync(string status)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByStatusAsync(status);
                if (orders?.Any() != true)
                {
                    return Response<IEnumerable<OrderDto>>.Fail("No Order For This status ");

                }
                var orderDtos = orders.Select(MapToOrderDto).ToList();
                return Response<IEnumerable<OrderDto>>.Success(orderDtos, $"{status} orders retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<OrderDto>>.Fail($"Error retrieving {status} orders: {ex.Message}");
            }
        }

        public async Task<Response<OrderDetailDto>> CreateOrderAsync(CreateOrderDto orderDto)
        {
            try
            {
                // Calculate order totals and validate products
                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();


                foreach (var item in orderDto.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                        return Response<OrderDetailDto>.Fail($"Product with ID {item.ProductId} not found");

                    if (product.StockQuantity < item.Quantity)
                        return Response<OrderDetailDto>.Fail($"Insufficient stock for product: {product.Name}");

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    totalAmount += orderItem.SubTotal;
                    orderItems.Add(orderItem);

                    // Update product stock
                    product.StockQuantity -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }


                var userid = _currentUserService.UserId;
                // Create order
                var order = new Order
                {
                    UserId = userid,
                    TotalAmount = totalAmount,
                    ShippingFee = orderDto.ShippingFee,
                    DiscountAmount = 0, // انا مخليها معتمده على الكبون لو فيه 
                    PaymentMethod = orderDto.PaymentMethod,
                    Status = OrderStatus.Pending,
                    OrderItems = orderItems,
                    OrderDate = DateTime.UtcNow
                };

                var createdOrder = await _orderRepository.AddAsync(order);
                var resultDto = MapToOrderDetailDto(createdOrder);
                return Response<OrderDetailDto>.Success(resultDto, "Order created successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDetailDto>.Fail($"Error creating order: {ex.Message}");
            }
        }



        public async Task<Response<OrderDetailDto>> CreateOrderFromCartAsync(string userId, CreateOrderFromCartDto orderDto)
        {
            try
            {
                //  نجيب كل الكارت ايتم بتاع المستخدم ال عامل تسجيل دخول
                var cartItems = await _cartRepository.GetUserCartAsync(userId);

                if (!cartItems.Any())
                    return Response<OrderDetailDto>.Fail("Cart is empty");

                // جديد Order نعمل  
                var order = new Order
                {
                    UserId = userId,
                    OrderNumber = GenerateOrderNumber(),
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                    ShippingFee = orderDto.ShippingFee,
                    PaymentMethod = orderDto.PaymentMethod
                };

               



                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                // هحول الكارت ايتم ل اوردر ايتم
                foreach (var cartItem in cartItems)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                        return Response<OrderDetailDto>.Fail($"Product with ID {cartItem.ProductId} not found");

                    if (product.StockQuantity < cartItem.Quantity)
                        return Response<OrderDetailDto>.Fail($"Insufficient stock for {product.Name}. Only {product.StockQuantity} available");

                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice,
                        Order = order
                    };

                    totalAmount += orderItem.SubTotal;
                    orderItems.Add(orderItem);

                    // نحدث مخزون المنتج
                    product.StockQuantity -= cartItem.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    await _productRepository.UpdateAsync(product);
                }

                //  نحسب التوتالات النهائية  
                order.TotalAmount = totalAmount;
                order.DiscountAmount = CalculateDiscount(totalAmount, orderDto.CouponCode);
                // order.FinalAmount هتتحسب أوتوماتيك   

                order.OrderItems = orderItems;



                // لو طريقة الدفع من المحفظة
                if (orderDto.PaymentMethod == PaymentMethod.Wallet)
                {
                    var hasBalance = await _walletService.HasSufficientBalanceAsync(userId, order.FinalAmount);
                    if (!hasBalance.Succeeded)
                        return Response<OrderDetailDto>.Fail("Insufficient wallet balance");

                    // نخصم من المحفظة
                    var deductResult = await _walletService.DeductBalanceAsync(
                        userId,
                        order.FinalAmount,
                        $"Payment for order #{order.OrderNumber}");

                    if (!deductResult.Succeeded)
                        return Response<OrderDetailDto>.Fail($"Payment failed: {deductResult.Message}");
                }

                //  نحفظ الطلب في الداتابيز
                var createdOrder = await _orderRepository.AddAsync(order);

                //  نفضي سلة المستخدم
                await _cartRepository.ClearUserCartAsync(userId);

                var resultDto = MapToOrderDetailDto(createdOrder);
                return Response<OrderDetailDto>.Success(resultDto, "Order created successfully from cart");
            }
            catch (Exception ex)
            {
                return Response<OrderDetailDto>.Fail($"Error creating order from cart: {ex.Message}");
            }
        }



        private string GenerateOrderNumber()
        {
            return $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private decimal CalculateDiscount(decimal totalAmount, string couponCode)
        {
            // منطق حساب الخصم -
            return 0m; // مؤقتاً
        }




        public async Task<Response<OrderDto>> UpdateOrderStatusAsync(int id, string status)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return Response<OrderDto>.Fail("Order not found");

                await _orderRepository.UpdateOrderStatusAsync(id, status);

                // إذا الطلب اتحول لـ
                // Delivered،
                // نحدث مبيعات البائعين
                if (status == "Delivered")
                {
                    await UpdateVendorsSales(order.Id);
                }

                var updatedOrder = await _orderRepository.GetByIdAsync(id);
                var orderDto = MapToOrderDto(updatedOrder);
                return Response<OrderDto>.Success(orderDto, "Order status updated successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDto>.Fail($"Error updating order status: {ex.Message}");
            }
        }

        public async Task<Response<OrderDto>> UpdateOrderAsync(int id, UpdateOrderDto orderDto)
        {
            try
            {
                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                    return Response<OrderDto>.Fail("Order not found");

                // Update properties
                existingOrder.Status = orderDto.Status;
                existingOrder.TrackingNumber = orderDto.TrackingNumber;
                existingOrder.ShippingFee = orderDto.ShippingFee;
                existingOrder.UpdatedAt = DateTime.UtcNow;

                await _orderRepository.UpdateAsync(existingOrder);
                var updatedDto = MapToOrderDto(existingOrder);
                return Response<OrderDto>.Success(updatedDto, "Order updated successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDto>.Fail($"Error updating order: {ex.Message}");
            }
        }

        public async Task<Response<bool>> CancelOrderAsync(int id)
        {
            try
            {

                var userId = _currentUserService.UserId;
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return Response<bool>.Fail("Order not found");

                if (order.UserId != userId)
                    return Response<bool>.Fail("You are not authorized to cancel this order");

                // مش محجوز او مش تم 
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                    return Response<bool>.Fail("Order cannot be cancelled at this stage");

                await _orderRepository.UpdateOrderStatusAsync(id, "Cancelled");
                return Response<bool>.Success(true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error cancelling order: {ex.Message}");
            }
        }

        public async Task<Response<OrderStatsDto>> GetOrderStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var totalOrders = await _orderRepository.GetOrdersCountAsync(startDate, endDate);
                var totalSales = await _orderRepository.GetTotalSalesAsync(startDate, endDate);
                var pendingOrders = await _orderRepository.GetOrdersByStatusAsync("Pending");
                var completedOrders = await _orderRepository.GetOrdersByStatusAsync("Delivered");

                var stats = new OrderStatsDto
                {
                    TotalOrders = totalOrders,
                    TotalSales = totalSales,
                    PendingOrders = pendingOrders.Count(),
                    CompletedOrders = completedOrders.Count(),
                    AverageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0
                };

                return Response<OrderStatsDto>.Success(stats, "Order statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderStatsDto>.Fail($"Error retrieving order statistics: {ex.Message}");
            }
        }

        public async Task<Response<decimal>> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var totalSales = await _orderRepository.GetTotalSalesAsync(startDate, endDate);
                return Response<decimal>.Success(totalSales, "Total sales retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Fail($"Error retrieving total sales: {ex.Message}");
            }
        }


        // Manual Mapping Methods
        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                UserName = GetUserName(),
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                FinalAmount = order.FinalAmount,
                Status = order.Status,
                StatusDisplay = order.Status.ToString(),
                OrderDate = order.OrderDate,
                TrackingNumber = order.TrackingNumber,
            };
        }

        private string GetUserName()
        {
            return _currentUserService.FullName;


        }


        private async Task UpdateVendorsSales(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
                if (order == null || !order.OrderItems.Any())
                    return;

                // Set علشان نتجنب التكرار
                var vendorIds = new HashSet<int>();

                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem.Product?.VendorId.HasValue == true)
                    {
                        vendorIds.Add(orderItem.Product.VendorId.Value);
                    }
                }

                // نحدث المبيعات لكل Vendor مرة واحدة
                foreach (var vendorId in vendorIds)
                {
                    await _vendorRepository.UpdateVendorSales(vendorId);
                }

               
            }
            catch (Exception ex)
            {
            }
        }
        private OrderDetailDto MapToOrderDetailDto(Order order)
        {
            return new OrderDetailDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                UserName = GetUserName(),
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                FinalAmount = order.FinalAmount,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                StatusDisplay = order.Status.ToString(),
                OrderDate = order.OrderDate,
                TrackingNumber = order?.TrackingNumber,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    ProductImage = oi.Product?.ImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SubTotal = oi.SubTotal
                }).ToList() ?? new List<OrderItemDto>(),
                //Payment = order.Payment != null ? new PaymentDto
                //{
                //    Id = order.Payment.Id,
                //    PaymentStatus = order.Payment.PaymentStatus,
                //    PaymentGateway = order.Payment.PaymentGateway,
                //    TransactionId = order.Payment.TransactionId,
                //    PaidDate = order.Payment.PaidDate
                //} : null,
                //Shipment = order.Shipment != null ? new ShipmentDto
                //{
                //    Id = order.Shipment.Id,
                //    CourierName = order.Shipment.CourierName,
                //    TrackingNumber = order.Shipment.TrackingNumber,
                //    ShippedDate = order.Shipment.ShippedDate,
                //    EstimatedDelivery = order.Shipment.EstimatedDelivery,
                //    DeliveredDate = order.Shipment.DeliveredDate
                //} : null
            };
        }
    }
}