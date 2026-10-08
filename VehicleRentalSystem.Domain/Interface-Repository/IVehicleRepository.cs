using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<Vehicle?> GetByIdWithDetailsAsync(int id);
        Task<Vehicle?> GetByRegistrationNumberAsync(string registrationNumber);
        Task<bool> RegistrationNumberExistsAsync(string registrationNumber);
        Task<IEnumerable<Vehicle>> GetAllWithDetailsAsync();
        Task<IEnumerable<Vehicle>> GetByBranchAsync(int branchId);
        Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status);

        Task AddImageAsync(VehicleImage image);
        Task<VehicleImage?> GetImageByIdAsync(int imageId);
        void DeleteImage(VehicleImage image);

        Task AddDocumentAsync(VehicleDocument document);
        Task<VehicleDocument?> GetDocumentByIdAsync(int documentId);
        void DeleteDocument(VehicleDocument document);
        Task<IEnumerable<VehicleDocument>> GetDocumentsExpiringBeforeAsync(DateOnly date);
    }
}
