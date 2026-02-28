using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.DTOs.deliveryagent;
using ECommerce.Core.DTOs.shipment;
using ECommerce.Core.Entities;
using ECommerce.Core.GenralResponse;
using ECommerce.Core.Interfaces;

namespace ECommerce.Service.Services
{
    public class DeliveryAgentService : IDeliveryAgentService
    {
        private readonly IDeliveryAgentRepository _deliveryAgentRepository;
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IGenericRepository<AuditLog> _auditLogRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeliveryAgentService(
            IDeliveryAgentRepository deliveryAgentRepository,
            IGenericRepository<ApplicationUser> userRepository,
            IShipmentRepository shipmentRepository,
            IGenericRepository<AuditLog> auditLogRepository,
            ICurrentUserService currentUserService)
        {
            _deliveryAgentRepository = deliveryAgentRepository;
            _userRepository = userRepository;
            _shipmentRepository = shipmentRepository;
            _auditLogRepository = auditLogRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Response<DeliveryAgentDetailDto>> GetDeliveryAgentByIdAsync(int id)
        {
            try
            {
                var deliveryAgent = await _deliveryAgentRepository.GetDeliveryAgentWithDetailsAsync(id);
                if (deliveryAgent == null)
                    return Response<DeliveryAgentDetailDto>.Fail("Delivery agent not found");

                var deliveryAgentDto = MapToDeliveryAgentDetailDto(deliveryAgent);
                return Response<DeliveryAgentDetailDto>.Success(deliveryAgentDto, "Delivery agent retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<DeliveryAgentDetailDto>.Fail($"Error retrieving delivery agent: {ex.Message}");
            }
        }

        public async Task<Response<DeliveryAgentDetailDto>> GetDeliveryAgentByUserIdAsync(string userId)
        {
            try
            {
                var deliveryAgent = await _deliveryAgentRepository.GetDeliveryAgentByUserIdAsync(userId);
                if (deliveryAgent == null)
                    return Response<DeliveryAgentDetailDto>.Fail("Delivery agent not found");

                var deliveryAgentDto = MapToDeliveryAgentDetailDto(deliveryAgent);
                return Response<DeliveryAgentDetailDto>.Success(deliveryAgentDto, "Delivery agent retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<DeliveryAgentDetailDto>.Fail($"Error retrieving delivery agent: {ex.Message}");
            }
        }

        public async Task<Response<DeliveryAgentDto>> GetCurrentDeliveryAgentAsync(string userId)
        {
            try
            {
                var deliveryAgent = await _deliveryAgentRepository.GetDeliveryAgentByUserIdAsync(userId);
                if (deliveryAgent == null)
                    return Response<DeliveryAgentDto>.Fail("Delivery agent profile not found");

                var deliveryAgentDto = MapToDeliveryAgentDto(deliveryAgent);
                return Response<DeliveryAgentDto>.Success(deliveryAgentDto, "Delivery agent profile retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<DeliveryAgentDto>.Fail($"Error retrieving delivery agent profile: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<DeliveryAgentDto>>> GetAllDeliveryAgentsAsync()
        {
            try
            {
                var deliveryAgents = await _deliveryAgentRepository.GetAllAsync();
                if (deliveryAgents == null)
                {
                    throw new Exception("not found an deliveryAgents ");
                }
                var deliveryAgentDtos = deliveryAgents.Select(MapToDeliveryAgentDto).ToList();
                return Response<IEnumerable<DeliveryAgentDto>>.Success(deliveryAgentDtos, "Delivery agents retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<DeliveryAgentDto>>.Fail($"Error retrieving delivery agents: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<DeliveryAgentDto>>> GetAvailableDeliveryAgentsAsync()
        {
            try
            {
                var deliveryAgents = await _deliveryAgentRepository.GetAvailableDeliveryAgentsAsync();
                if (deliveryAgents == null)
                {
                    throw new Exception("not found an Available deliveryAgents");
                }
                var deliveryAgentDtos = deliveryAgents.Select(MapToDeliveryAgentDto).ToList();
                return Response<IEnumerable<DeliveryAgentDto>>.Success(deliveryAgentDtos, "Available delivery agents retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<DeliveryAgentDto>>.Fail($"Error retrieving available delivery agents: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<DeliveryAgentDto>>> GetDeliveryAgentsByStatusAsync(string status)
        {
            try
            {
                var deliveryAgents = await _deliveryAgentRepository.GetDeliveryAgentsByStatusAsync(status);
                var deliveryAgentDtos = deliveryAgents.Select(MapToDeliveryAgentDto).ToList();
                return Response<IEnumerable<DeliveryAgentDto>>.Success(deliveryAgentDtos, $"{status} delivery agents retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<DeliveryAgentDto>>.Fail($"Error retrieving {status} delivery agents: {ex.Message}");
            }
        }

        public async Task<Response<DeliveryAgentDto>> CreateDeliveryAgentAsync(CreateDeliveryAgentDto deliveryAgentDto)
        {
            try
            {
                // نتأكد إن المستخدم موجود
                var user = await _userRepository.GetByIdAsync(deliveryAgentDto.UserId);
                if (user == null)
                    return Response<DeliveryAgentDto>.Fail("User not found");

                // نتأكد إن المستخدم مش مندوب توصيل بالفعل
                var existingAgent = await _deliveryAgentRepository.GetDeliveryAgentByUserIdAsync(deliveryAgentDto.UserId);
                if (existingAgent != null)
                    return Response<DeliveryAgentDto>.Fail("User is already a delivery agent");

                // نتأكد إن رقم المركبة فريد
                var vehicleNumberExists = await _deliveryAgentRepository.IsVehicleNumberUniqueAsync(deliveryAgentDto.VehicleNumber);
                if (!vehicleNumberExists)
                    return Response<DeliveryAgentDto>.Fail("Vehicle number already exists");

                var deliveryAgent = new DeliveryAgent
                {
                    UserId = deliveryAgentDto.UserId,
                    VehicleNumber = deliveryAgentDto.VehicleNumber,
                    CurrentStatus = deliveryAgentDto.CurrentStatus,
                    CreatedAt = DateTime.UtcNow
                };

                var createdDeliveryAgent = await _deliveryAgentRepository.AddAsync(deliveryAgent);

                await CreateAuditLog("DeliveryAgent", "Create",
                    $"Delivery agent created for user {user.FullName}. Vehicle: {deliveryAgentDto.VehicleNumber}",
                    _currentUserService.UserId);

                var resultDto = MapToDeliveryAgentDto(createdDeliveryAgent);
                return Response<DeliveryAgentDto>.Success(resultDto, "Delivery agent created successfully");
            }
            catch (Exception ex)
            {
                return Response<DeliveryAgentDto>.Fail($"Error creating delivery agent: {ex.Message}");
            }
        }

        public async Task<Response<DeliveryAgentDto>> UpdateDeliveryAgentAsync(int id, UpdateDeliveryAgentDto deliveryAgentDto)
        {
            try
            {
                var existingDeliveryAgent = await _deliveryAgentRepository.GetByIdAsync(id);
                if (existingDeliveryAgent == null)
                    return Response<DeliveryAgentDto>.Fail("Delivery agent not found");

                // نتأكد إن رقم المركبة فريد
                if (deliveryAgentDto.VehicleNumber != existingDeliveryAgent.VehicleNumber)
                {
                    var vehicleNumberExists = await _deliveryAgentRepository.IsVehicleNumberUniqueAsync(deliveryAgentDto.VehicleNumber, id);
                    if (!vehicleNumberExists)
                        return Response<DeliveryAgentDto>.Fail("Vehicle number already exists");
                }

                // نحدث الخصائص
                existingDeliveryAgent.VehicleNumber = deliveryAgentDto.VehicleNumber;
                existingDeliveryAgent.CurrentStatus = deliveryAgentDto.CurrentStatus;
                existingDeliveryAgent.UpdatedAt = DateTime.UtcNow;

                await _deliveryAgentRepository.UpdateAsync(existingDeliveryAgent);

                await CreateAuditLog("DeliveryAgent", "Update",
                    $"Delivery agent updated. New vehicle: {deliveryAgentDto.VehicleNumber}, Status: {deliveryAgentDto.CurrentStatus}",
                    _currentUserService.UserId);

                var updatedDto = MapToDeliveryAgentDto(existingDeliveryAgent);
                return Response<DeliveryAgentDto>.Success(updatedDto, "Delivery agent updated successfully");
            }
            catch (Exception ex)
            {
                return Response<DeliveryAgentDto>.Fail($"Error updating delivery agent: {ex.Message}");
            }
        }

        public async Task<Response<bool>> DeleteDeliveryAgentAsync(int id)
        {
            try
            {
                var deliveryAgent = await _deliveryAgentRepository.GetByIdAsync(id);
                if (deliveryAgent == null)
                    return Response<bool>.Fail("Delivery agent not found");

                // نتأكد إن مفيش شحنات نشطة للمندوب
                var activeShipments = await _deliveryAgentRepository.GetActiveShipmentsCountAsync(id);
                if (activeShipments > 0)
                    return Response<bool>.Fail("Cannot delete delivery agent with active shipments");

                await _deliveryAgentRepository.DeleteAsync(deliveryAgent);

                await CreateAuditLog("DeliveryAgent", "Delete",
                    $"Delivery agent deleted: {deliveryAgent.User?.FullName}",
                    _currentUserService.UserId);

                return Response<bool>.Success(true, "Delivery agent deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error deleting delivery agent: {ex.Message}");
            }
        }

        public async Task<Response<DeliveryAgentDto>> UpdateDeliveryAgentStatusAsync(int id, string status)
        {
            try
            {
                var deliveryAgent = await _deliveryAgentRepository.GetByIdAsync(id);
                if (deliveryAgent == null)
                    return Response<DeliveryAgentDto>.Fail("Delivery agent not found");

                await _deliveryAgentRepository.UpdateDeliveryAgentStatusAsync(id, status);

                await CreateAuditLog("DeliveryAgent", "UpdateStatus",
                    $"Delivery agent status updated to: {status}",
                    _currentUserService.UserId);

                var updatedDeliveryAgent = await _deliveryAgentRepository.GetByIdAsync(id);
                var deliveryAgentDto = MapToDeliveryAgentDto(updatedDeliveryAgent);
                return Response<DeliveryAgentDto>.Success(deliveryAgentDto, "Delivery agent status updated successfully");
            }
            catch (Exception ex)
            {
                return Response<DeliveryAgentDto>.Fail($"Error updating delivery agent status: {ex.Message}");
            }
        }

        public async Task<Response<bool>> AssignShipmentToAgentAsync(int shipmentId, int deliveryAgentId)
        {
            try
            {
                var deliveryAgent = await _deliveryAgentRepository.GetByIdAsync(deliveryAgentId);
                if (deliveryAgent == null)
                    return Response<bool>.Fail("Delivery agent not found");

                // نتأكد إن المندوب متاح
                if (deliveryAgent.CurrentStatus != "Available")
                    return Response<bool>.Fail("Delivery agent is not available");

                await _shipmentRepository.AssignDeliveryAgentAsync(shipmentId, deliveryAgentId);

                //Busy نحدث حالة المندوب لـ   
                await _deliveryAgentRepository.UpdateDeliveryAgentStatusAsync(deliveryAgentId, "Busy");

                await CreateAuditLog("DeliveryAgent", "AssignShipment",
                    $"Shipment #{shipmentId} assigned to delivery agent {deliveryAgent.User?.FullName}",
                    _currentUserService.UserId);

                return Response<bool>.Success(true, "Shipment assigned to delivery agent successfully");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail($"Error assigning shipment to delivery agent: {ex.Message}");
            }
        }

        public async Task<Response<DeliveryAgentStatsDto>> GetDeliveryAgentStatsAsync()
        {
            try
            {
                var allAgents = await _deliveryAgentRepository.GetAllAsync();
                var availableAgents = await _deliveryAgentRepository.GetAvailableDeliveryAgentsAsync();
                var busyAgents = await _deliveryAgentRepository.GetBusyDeliveryAgentsAsync();
                var offlineAgents = await _deliveryAgentRepository.GetDeliveryAgentsByStatusAsync("Offline");

                var totalActiveShipments = 0;
                foreach (var agent in allAgents)
                {
                    totalActiveShipments += await _deliveryAgentRepository.GetActiveShipmentsCountAsync(agent.Id);
                }

                var stats = new DeliveryAgentStatsDto
                {
                    TotalAgents = allAgents.Count(),
                    AvailableAgents = availableAgents.Count(),
                    BusyAgents = busyAgents.Count(),
                    OfflineAgents = offlineAgents.Count(),
                    TotalActiveShipments = totalActiveShipments,
                    AverageShipmentsPerAgent = allAgents.Any() ? (double)totalActiveShipments / allAgents.Count() : 0
                };

                return Response<DeliveryAgentStatsDto>.Success(stats, "Delivery agent statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<DeliveryAgentStatsDto>.Fail($"Error retrieving delivery agent statistics: {ex.Message}");
            }
        }

        public async Task<Response<int>> GetActiveShipmentsCountAsync(int deliveryAgentId)
        {
            try
            {
                var count = await _deliveryAgentRepository.GetActiveShipmentsCountAsync(deliveryAgentId);
                return Response<int>.Success(count, "Active shipments count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<int>.Fail($"Error retrieving active shipments count: {ex.Message}");
            }
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
        private DeliveryAgentDto MapToDeliveryAgentDto(DeliveryAgent deliveryAgent)
        {
            return new DeliveryAgentDto
            {
                Id = deliveryAgent.Id,
                UserId = deliveryAgent.UserId,
                UserName = deliveryAgent.User?.FullName,
                UserEmail = deliveryAgent.User?.Email,
                UserPhone = deliveryAgent.User?.PhoneNumber,
                VehicleNumber = deliveryAgent.VehicleNumber,
                CurrentStatus = deliveryAgent.CurrentStatus,
                ActiveShipmentsCount = deliveryAgent.Shipments?.Count(s => s.DeliveredDate == null) ?? 0,
                CreatedAt = deliveryAgent.CreatedAt,
                UpdatedAt = deliveryAgent.UpdatedAt
            };
        }

        private DeliveryAgentDetailDto MapToDeliveryAgentDetailDto(DeliveryAgent deliveryAgent)
        {
            return new DeliveryAgentDetailDto
            {
                Id = deliveryAgent.Id,
                UserId = deliveryAgent.UserId,
                UserName = deliveryAgent.User?.FullName,
                UserEmail = deliveryAgent.User?.Email,
                UserPhone = deliveryAgent.User?.PhoneNumber,
                VehicleNumber = deliveryAgent.VehicleNumber,
                CurrentStatus = deliveryAgent.CurrentStatus,
                ActiveShipmentsCount = deliveryAgent.Shipments?.Count(s => s.DeliveredDate == null) ?? 0,
                TotalShipmentsCount = deliveryAgent.Shipments?.Count ?? 0,
                ActiveShipments = deliveryAgent.Shipments?
                    .Where(s => s.DeliveredDate == null)
                    .Select(s => new ShipmentDto
                    {
                        Id = s.Id,
                        OrderId = s.OrderId,
                        OrderNumber = s.Order?.OrderNumber,
                        CourierName = s.CourierName,
                        TrackingNumber = s.TrackingNumber,
                        ShippedDate = s.ShippedDate,
                        EstimatedDelivery = s.EstimatedDelivery,
                        Status = GetShipmentStatus(s)
                    }).ToList() ?? new List<ShipmentDto>(),
                CreatedAt = deliveryAgent.CreatedAt,
                UpdatedAt = deliveryAgent.UpdatedAt
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