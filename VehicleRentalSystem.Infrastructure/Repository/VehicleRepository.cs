using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context) : base(context) { }

        private IQueryable<Vehicle> WithDetails() =>
            _dbSet.Include(v => v.Category)
                  .Include(v => v.Branch)
                  .Include(v => v.Images)
                  .Include(v => v.Documents);

        public async Task<Vehicle?> GetByIdWithDetailsAsync(int id) =>
            await WithDetails().FirstOrDefaultAsync(v => v.Id == id);

        public async Task<Vehicle?> GetByRegistrationNumberAsync(string registrationNumber) =>
            await _dbSet.FirstOrDefaultAsync(v => v.RegistrationNumber == registrationNumber);

        public async Task<bool> RegistrationNumberExistsAsync(string registrationNumber) =>
            await _dbSet.AnyAsync(v => v.RegistrationNumber == registrationNumber);

        public async Task<IEnumerable<Vehicle>> GetAllWithDetailsAsync() =>
            await WithDetails().AsNoTracking().OrderBy(v => v.RegistrationNumber).ToListAsync();

        public async Task<IEnumerable<Vehicle>> GetByBranchAsync(int branchId) =>
            await WithDetails().AsNoTracking().Where(v => v.BranchId == branchId).ToListAsync();

        public async Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status) =>
            await WithDetails().AsNoTracking().Where(v => v.Status == status).ToListAsync();

        // Images
        public async Task AddImageAsync(VehicleImage image) =>
            await _context.VehicleImages.AddAsync(image);

        public async Task<VehicleImage?> GetImageByIdAsync(int imageId) =>
            await _context.VehicleImages.FindAsync(imageId);

        public void DeleteImage(VehicleImage image) => _context.VehicleImages.Remove(image);

        // Documents
        public async Task AddDocumentAsync(VehicleDocument document) =>
            await _context.VehicleDocuments.AddAsync(document);

        public async Task<VehicleDocument?> GetDocumentByIdAsync(int documentId) =>
            await _context.VehicleDocuments.FindAsync(documentId);

        public void DeleteDocument(VehicleDocument document) =>
            _context.VehicleDocuments.Remove(document);

        public async Task<IEnumerable<VehicleDocument>> GetDocumentsExpiringBeforeAsync(DateOnly date) =>
            await _context.VehicleDocuments.AsNoTracking()
                .Include(d => d.Vehicle)
                .Where(d => d.ExpiryDate <= date)
                .OrderBy(d => d.ExpiryDate)
                .ToListAsync();
    }
}
