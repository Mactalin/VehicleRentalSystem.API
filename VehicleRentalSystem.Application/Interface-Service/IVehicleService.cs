using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Vehicle;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllAsync(VehicleStatus? status, int? branchId, int? categoryId, bool includeInactive = false);
        Task<VehicleDto> GetByIdAsync(int id);
        Task<VehicleDto> CreateAsync(SaveVehicleDto dto);
        Task<VehicleDto> UpdateAsync(int id, SaveVehicleDto dto);
        Task UpdateStatusAsync(int id, VehicleStatus status);   // Mac's booking module can call this
        Task DeleteAsync(int id);

        Task<VehicleImageDto> AddImageAsync(int vehicleId, AddVehicleImageDto dto);
        Task SetPrimaryImageAsync(int vehicleId, int imageId);
        Task DeleteImageAsync(int vehicleId, int imageId);

        Task<VehicleDocumentDto> AddDocumentAsync(int vehicleId, SaveVehicleDocumentDto dto);
        Task<VehicleDocumentDto> UpdateDocumentAsync(int vehicleId, int documentId, SaveVehicleDocumentDto dto);
        Task DeleteDocumentAsync(int vehicleId, int documentId);
        Task<IEnumerable<ExpiringDocumentDto>> GetExpiringDocumentsAsync(int withinDays = 30);
    }
}
