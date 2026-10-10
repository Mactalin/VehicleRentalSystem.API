using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public int? BranchId { get; set; }
    public string? City { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public IReadOnlyList<int>? FeatureIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// Anbu owns the Vehicle entity; Mac only needs the search + availability query.
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
