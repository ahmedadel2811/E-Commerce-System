using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.order;
using ECommerce.Core.DTOs.shipment;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryAgentRepository _deliveryAgentRepository;
        private readonly IGenericRepository<AuditLog> _auditLogRepository;

        public ShipmentService(
            IShipmentRepository shipmentRepository,
            IOrderRepository orderRepository,

            IGenericRepository<AuditLog> auditLogRepository,
            IDeliveryAgentRepository deliveryAgentRepository)
        {
            _shipmentRepository = shipmentRepository;
            _orderRepository = orderRepository;

            _auditLogRepository = auditLogRepository;
            _deliveryAgentRepository = deliveryAgentRepository;
        }

        public async Task<Response<ShipmentDetailDto>> GetShipmentByIdAsync(int id)
        {
            try
            {
                var shipment = await _shipmentRepository.GetShipmentWithDetailsAsync(id);
                if (shipment == null)
                    return Response<ShipmentDetailDto>.Fail("Shipment not found");

                var shipmentDto = MapToShipmentDetailDto(shipment);
                return Response<ShipmentDetailDto>.Success(shipmentDto, "Shipment retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ShipmentDetailDto>.Fail($"Error retrieving shipment: {ex.Message}");
            }
        }

        public async Task<Response<ShipmentTrackingDto>> GetShipmentByTrackingNumberAsync(string trackingNumber)
        {
            try
            {
                var shipment = await _shipmentRepository.GetShipmentByTrackingNumberAsync(trackingNumber);
                if (shipment == null)
                    return Response<ShipmentTrackingDto>.Fail("Shipment not found");

                var trackingDto = MapToShipmentTrackingDto(shipment);
                return Response<ShipmentTrackingDto>.Success(trackingDto, "Shipment tracking information retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ShipmentTrackingDto>.Fail($"Error retrieving shipment tracking: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ShipmentDto>>> GetAllShipmentsAsync()
        {
            try
            {
                var shipments = await _shipmentRepository.GetAllAsync();
                var shipmentDtos = shipments.Select(MapToShipmentDto).ToList();
                return Response<IEnumerable<ShipmentDto>>.Success(shipmentDtos, "Shipments retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ShipmentDto>>.Fail($"Error retrieving shipments: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ShipmentDto>>> GetShipmentsByStatusAsync(string status)
        {
            try
            {
                var shipments = await _shipmentRepository.GetShipmentsByStatusAsync(status);
                var shipmentDtos = shipments.Select(MapToShipmentDto).ToList();
                return Response<IEnumerable<ShipmentDto>>.Success(shipmentDtos, $"{status} shipments retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ShipmentDto>>.Fail($"Error retrieving {status} shipments: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ShipmentDto>>> GetShipmentsByDeliveryAgentAsync(int deliveryAgentId)
        {
            try
            {
                var shipments = await _shipmentRepository.GetShipmentsByDeliveryAgentAsync(deliveryAgentId);
                var shipmentDtos = shipments.Select(MapToShipmentDto).ToList();
                return Response<IEnumerable<ShipmentDto>>.Success(shipmentDtos, "Delivery agent shipments retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ShipmentDto>>.Fail($"Error retrieving delivery agent shipments: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<ShipmentDto>>> GetPendingShipmentsAsync()
        {
            try
            {
                var shipments = await _shipmentRepository.GetPendingShipmentsAsync();
                var shipmentDtos = shipments.Select(MapToShipmentDto).ToList();
                return Response<IEnumerable<ShipmentDto>>.Success(shipmentDtos, "Pending shipments retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ShipmentDto>>.Fail($"Error retrieving pending shipments: {ex.Message}");
            }
        }

        public async Task<Response<ShipmentDto>> CreateShipmentAsync(CreateShipmentDto shipmentDto)
        {
            try
            {
                // نتأكد إن الطلب موجود
                var order = await _orderRepository.GetByIdAsync(shipmentDto.OrderId);
                if (order == null)
                    return Response<ShipmentDto>.Fail("Order not found");

                // نتأكد إن الطلب مدفوع  
                // لو حاله الطلب لا تساوى انه تم الدفع
                //  مش هقدر اعمل شحنه لانه متدفعش  

                if (order.Status != Core.Enums.OrderStatus.Confirmed )
                    return Response<ShipmentDto>.Fail("Cannot create shipment for unpaid order");

                // نتأكد إن مفيش
                // shipment
                // موجودة للطلب
                var existingShipment = await _shipmentRepository.FindAsync(s => s.OrderId == shipmentDto.OrderId);
                if (existingShipment.Any())
                    return Response<ShipmentDto>.Fail("Shipment already exists for this order");

                // نتأكد إن
                // tracking number
                // فريد
                if (!string.IsNullOrEmpty(shipmentDto.TrackingNumber)) // الشرط هيتحقق لو جايلى تراكنج نمبر
                {
                    var trackingExists = await _shipmentRepository.TrackingNumberExistsAsync(shipmentDto.TrackingNumber);
                    if (trackingExists)
                        return Response<ShipmentDto>.Fail("Tracking number already exists");
                }

                // نتأكد إن
                // delivery agent
                // موجود لو متاح
                if (shipmentDto.DeliveryAgentId.HasValue)
                {
                    var deliveryAgent = await _deliveryAgentRepository.GetByIdAsync(shipmentDto.DeliveryAgentId.Value);
                    if (deliveryAgent == null)
                        return Response<ShipmentDto>.Fail("Delivery agent not found");
                }

                var shipment = new Shipment
                {
                    OrderId = shipmentDto.OrderId,
                    CourierName = shipmentDto.CourierName,
                    TrackingNumber = shipmentDto.TrackingNumber ?? GenerateTrackingNumber(),
                    EstimatedDelivery = shipmentDto.EstimatedDelivery,
                    DeliveryAgentId = shipmentDto.DeliveryAgentId,
                    CreatedAt = DateTime.UtcNow
                };

                var createdShipment = await _shipmentRepository.AddAsync(shipment);

                // نحدث حالة الطلب
                await _orderRepository.UpdateOrderStatusAsync(order.Id, "Shipped");

                // نعمل
                // audit log
                await CreateAuditLog("Shipment", "Create",
                    $"Shipment created for order #{order.OrderNumber}. Tracking: {shipment.TrackingNumber}",
                    order.UserId);

                var resultDto = MapToShipmentDto(createdShipment);
                return Response<ShipmentDto>.Success(resultDto, "Shipment created successfully");
            }
            catch (Exception ex)
            {
                return Response<ShipmentDto>.Fail($"Error creating shipment: {ex.Message}");
            }
        }

        public async Task<Response<ShipmentDto>> UpdateShipmentAsync(int id, UpdateShipmentDto shipmentDto)
        {
            try
            {
                var existingShipment = await _shipmentRepository.GetByIdAsync(id);
                if (existingShipment == null)
                    return Response<ShipmentDto>.Fail("Shipment not found");

                // نتأكد إن tracking number فريد
                if (!string.IsNullOrEmpty(shipmentDto.TrackingNumber) &&
                    shipmentDto.TrackingNumber != existingShipment.TrackingNumber)
                {
                    var trackingExists = await _shipmentRepository.TrackingNumberExistsAsync(shipmentDto.TrackingNumber);
                    if (trackingExists)
                        return Response<ShipmentDto>.Fail("Tracking number already exists");
                }

                // نتأكد إن
                // delivery agent
                // موجود لو متاح
                if (shipmentDto.DeliveryAgentId.HasValue)
                {
                    var deliveryAgent = await _deliveryAgentRepository.GetByIdAsync(shipmentDto.DeliveryAgentId.Value);
                    if (deliveryAgent == null)
                        return Response<ShipmentDto>.Fail("Delivery agent not found");
                }

                // نحدث الخصائص
                existingShipment.CourierName = shipmentDto.CourierName;
                existingShipment.TrackingNumber = shipmentDto.TrackingNumber ?? existingShipment.TrackingNumber;
                existingShipment.EstimatedDelivery = shipmentDto.EstimatedDelivery;
                existingShipment.DeliveryAgentId = shipmentDto.DeliveryAgentId;
                existingShipment.UpdatedAt = DateTime.UtcNow;

                await _shipmentRepository.UpdateAsync(existingShipment);

                await CreateAuditLog("Shipment", "Update",
                    $"Shipment updated for tracking: {existingShipment.TrackingNumber}",
                    existingShipment.Order?.UserId);

                var updatedDto = MapToShipmentDto(existingShipment);
                return Response<ShipmentDto>.Success(updatedDto, "Shipment updated successfully");
            }
            catch (Exception ex)
            {
                return Response<ShipmentDto>.Fail($"Error updating shipment: {ex.Message}");
            }
        }

        public async Task<Response<ShipmentDto>> UpdateShipmentStatusAsync(int id, ShipmentStatusDto statusDto)
        {
            try
            {
                var shipment = await _shipmentRepository.GetByIdAsync(id);
                if (shipment == null)
                    return Response<ShipmentDto>.Fail("Shipment not found");

                await _shipmentRepository.UpdateShipmentStatusAsync(id, statusDto.ShippedDate, statusDto.DeliveredDate, statusDto.TrackingNumber);

                // نحدث حالة الطلب بناءً على حالة الشحنة
                if (statusDto.DeliveredDate.HasValue)
                {
                    await _orderRepository.UpdateOrderStatusAsync((int)shipment.OrderId, "Delivered");
                }
                else if (statusDto.ShippedDate.HasValue)
                {
                    await _orderRepository.UpdateOrderStatusAsync((int)shipment.OrderId, "Shipped");
                }

                var updatedShipment = await _shipmentRepository.GetByIdAsync(id);
                var shipmentDto = MapToShipmentDto(updatedShipment);
                return Response<ShipmentDto>.Success(shipmentDto, "Shipment status updated successfully");
            }
            catch (Exception ex)
            {
                return Response<ShipmentDto>.Fail($"Error updating shipment status: {ex.Message}");
            }
        }

        public async Task<Response<bool>> AssignDeliveryAgentAsync(int shipmentId, int deliveryAgentId)
        {
            try
            {
                var deliveryAgent = await _deliveryAgentRepository.GetByIdAsync(deliveryAgentId);
                if (deliveryAgent == null)
                    return Response<bool>.Fail("Delivery agent not found");

                await _shipmentRepository.AssignDeliveryAgentAsync(shipmentId, deliveryAgentId);

                await CreateAuditLog("Shipment", "AssignAgent",
                    $"Delivery agent {deliveryAgent.User.FullName} assigned to shipment",
                    null);

                return Response<bool>.Success(true, "Delivery agent assigned successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error assigning delivery agent: {ex.Message}");
            }
        }

        public async Task<Response<bool>> MarkAsShippedAsync(int shipmentId, string trackingNumber = null)
        {
            try
            {
                await _shipmentRepository.UpdateShipmentStatusAsync(shipmentId, DateTime.UtcNow, null, trackingNumber);

                var shipment = await _shipmentRepository.GetByIdAsync(shipmentId);
                await _orderRepository.UpdateOrderStatusAsync((int)shipment.OrderId, "Shipped");

                await CreateAuditLog("Shipment", "Shipped",
                    $"Shipment marked as shipped. Tracking: {trackingNumber}",
                    shipment.Order?.UserId);

                return Response<bool>.Success(true, "Shipment marked as shipped successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error marking shipment as shipped: {ex.Message}");
            }
        }

        public async Task<Response<bool>> MarkAsDeliveredAsync(int shipmentId)
        {
            try
            {
                await _shipmentRepository.UpdateShipmentStatusAsync(shipmentId, null, DateTime.UtcNow);

                var shipment = await _shipmentRepository.GetByIdAsync(shipmentId);
                await _orderRepository.UpdateOrderStatusAsync((int)shipment.OrderId, "Delivered");

                await CreateAuditLog("Shipment", "Delivered",
                    $"Shipment marked as delivered",
                    shipment.Order?.UserId);

                return Response<bool>.Success(true, "Shipment marked as delivered successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error marking shipment as delivered: {ex.Message}");
            }
        }

        public async Task<Response<ShipmentStatsDto>> GetShipmentStatsAsync()
        {
            try
            {
                var allShipments = await _shipmentRepository.GetAllAsync();
                var pendingShipments = await _shipmentRepository.GetPendingShipmentsAsync();
                var shippedShipments = await _shipmentRepository.GetShipmentsByStatusAsync("shipped");
                var deliveredShipments = await _shipmentRepository.GetShipmentsByStatusAsync("delivered");

                var lateShipments = deliveredShipments.Where(s =>
                    s.DeliveredDate > s.EstimatedDelivery).Count();

                var onTimeDeliveryRate = deliveredShipments.Any() ?
                    (decimal)(deliveredShipments.Count() - lateShipments) / deliveredShipments.Count() * 100 : 0;

                var stats = new ShipmentStatsDto
                {
                    TotalShipments = allShipments.Count(),
                    PendingShipments = pendingShipments.Count(),
                    ShippedShipments = shippedShipments.Count(),
                    DeliveredShipments = deliveredShipments.Count(),
                    LateShipments = lateShipments,
                    OnTimeDeliveryRate = onTimeDeliveryRate
                };

                return Response<ShipmentStatsDto>.Success(stats, "Shipment statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ShipmentStatsDto>.Fail($"Error retrieving shipment statistics: {ex.Message}");
            }
        }

        public async Task<Response<bool>> CanCreateShipmentAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return Response<bool>.Success(false, "Order not found");

                // نتأكد إن الطلب مدفوع ومش متعامل معاه قبل كده
                var existingShipment = await _shipmentRepository.FindAsync(s => s.OrderId == orderId);

                var canCreate = (order.Status == Core.Enums.OrderStatus.Confirmed || order.Status == Core.Enums.OrderStatus.Pending) &&
                               !existingShipment.Any();

                return Response<bool>.Success(canCreate, "Shipment creation check completed");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error checking shipment creation: {ex.Message}");
            }
        }

        private string GenerateTrackingNumber()
        {
            return $"TRK{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
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
        private ShipmentDto MapToShipmentDto(Shipment shipment)
        {
            var status = GetShipmentStatus(shipment);

            return new ShipmentDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                OrderNumber = shipment.Order?.OrderNumber,
                CourierName = shipment.CourierName,
                TrackingNumber = shipment.TrackingNumber,
                ShippedDate = shipment.ShippedDate,
                EstimatedDelivery = shipment.EstimatedDelivery,
                DeliveredDate = shipment.DeliveredDate,
                Status = shipment.Order?.Status.ToString(),
                DeliveryAgentId = shipment.DeliveryAgentId,
                DeliveryAgentName = shipment.DeliveryAgent?.User?.FullName,
                CreatedAt = shipment.CreatedAt
            };
        }

        private ShipmentDetailDto MapToShipmentDetailDto(Shipment shipment)
        {
            var status = GetShipmentStatus(shipment);

            return new ShipmentDetailDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                OrderNumber = shipment.Order?.OrderNumber,
                CourierName = shipment.CourierName,
                TrackingNumber = shipment.TrackingNumber,
                ShippedDate = shipment.ShippedDate,
                EstimatedDelivery = shipment.EstimatedDelivery,
                DeliveredDate = shipment.DeliveredDate,
                Status = status,
                DeliveryAgentId = shipment.DeliveryAgentId,
                DeliveryAgentName = shipment.DeliveryAgent?.User?.FullName,
                DeliveryAgentPhone = shipment.DeliveryAgent?.User?.PhoneNumber,
                Order = shipment.Order != null ? new OrderDto
                {
                    Id = shipment.Order.Id,
                    OrderNumber = shipment.Order.OrderNumber,
                    TotalAmount = shipment.Order.TotalAmount,
                    FinalAmount = shipment.Order.FinalAmount,
                    Status = shipment.Order.Status
                } : null,
                OrderItems = shipment.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SubTotal = oi.SubTotal
                }).ToList() ?? new List<OrderItemDto>(),
                CreatedAt = shipment.CreatedAt
            };
        }

        private ShipmentTrackingDto MapToShipmentTrackingDto(Shipment shipment)
        {
            var status = GetShipmentStatus(shipment);

            return new ShipmentTrackingDto
            {
                TrackingNumber = shipment.TrackingNumber,
                Status = status,
                CourierName = shipment.CourierName,
                ShippedDate = shipment.ShippedDate,
                EstimatedDelivery = shipment.EstimatedDelivery,
                DeliveredDate = shipment.DeliveredDate,
                Order = shipment.Order != null ? new OrderDto
                {
                    Id = shipment.Order.Id,
                    OrderNumber = shipment.Order.OrderNumber,
                    FinalAmount = shipment.Order.FinalAmount,
                    Status = shipment.Order.Status
                } : null,
                DeliveryAgentName = shipment.DeliveryAgent?.User?.FullName,
                DeliveryAgentPhone = shipment.DeliveryAgent?.User?.PhoneNumber
            };
        }

        private string GetShipmentStatus(Shipment shipment)
        {
            if (shipment.DeliveredDate.HasValue)
                return "Delivered";
            else if (shipment.ShippedDate.HasValue)
                return "Shipped";
            else
                return "Pending";
        }
    }
}