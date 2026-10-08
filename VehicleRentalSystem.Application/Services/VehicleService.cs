using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Vehicle;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicles;
        private readonly ICategoryRepository _categories;
        private readonly IBranchRepository _branches;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository vehicles, ICategoryRepository categories,
                              IBranchRepository branches, IMapper mapper)
        {
            _vehicles = vehicles;
            _categories = categories;
            _branches = branches;
            _mapper = mapper;
        }

        // ---------- Vehicles ----------

        public async Task<IEnumerable<VehicleDto>> GetAllAsync(
            VehicleStatus? status, int? branchId, int? categoryId, bool includeInactive = false)
        {
            IEnumerable<Vehicle> list = branchId.HasValue
                ? await _vehicles.GetByBranchAsync(branchId.Value)
                : status.HasValue
                    ? await _vehicles.GetByStatusAsync(status.Value)
                    : await _vehicles.GetAllWithDetailsAsync();

            if (status.HasValue) list = list.Where(v => v.Status == status.Value);
            if (categoryId.HasValue) list = list.Where(v => v.CategoryId == categoryId.Value);
            if (!includeInactive) list = list.Where(v => v.IsActive);

            return _mapper.Map<IEnumerable<VehicleDto>>(list);
        }

        public async Task<VehicleDto> GetByIdAsync(int id)
        {
            var vehicle = await _vehicles.GetByIdWithDetailsAsync(id)
                          ?? throw new NotFoundException("Vehicle not found.");
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<VehicleDto> CreateAsync(SaveVehicleDto dto)
        {
            var regNo = NormalizeRegNo(dto.RegistrationNumber);
            if (await _vehicles.RegistrationNumberExistsAsync(regNo))
                throw new ConflictException($"A vehicle with registration number {regNo} already exists.");

            await EnsureCategoryAndBranchAsync(dto);

            var vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.RegistrationNumber = regNo;
            vehicle.Status = VehicleStatus.Available;

            await _vehicles.AddAsync(vehicle);
            await _vehicles.SaveChangesAsync();

            return await GetByIdAsync(vehicle.Id);
        }

        public async Task<VehicleDto> UpdateAsync(int id, SaveVehicleDto dto)
        {
            var vehicle = await _vehicles.GetByIdAsync(id) ?? throw new NotFoundException("Vehicle not found.");

            var regNo = NormalizeRegNo(dto.RegistrationNumber);
            if (regNo != vehicle.RegistrationNumber && await _vehicles.RegistrationNumberExistsAsync(regNo))
                throw new ConflictException($"A vehicle with registration number {regNo} already exists.");

            await EnsureCategoryAndBranchAsync(dto);

            _mapper.Map(dto, vehicle);
            vehicle.RegistrationNumber = regNo;

            await _vehicles.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task UpdateStatusAsync(int id, VehicleStatus status)
        {
            var vehicle = await _vehicles.GetByIdAsync(id) ?? throw new NotFoundException("Vehicle not found.");

            if (!vehicle.IsActive)
                throw new BadRequestException("This vehicle is deactivated.");

            if (status == VehicleStatus.Maintenance &&
                vehicle.Status is VehicleStatus.Rented or VehicleStatus.Reserved)
                throw new BadRequestException("A rented or reserved vehicle cannot be moved to maintenance.");

            vehicle.Status = status;
            await _vehicles.SaveChangesAsync();
        }

        // Soft delete, because bookings will reference vehicles
        public async Task DeleteAsync(int id)
        {
            var vehicle = await _vehicles.GetByIdAsync(id) ?? throw new NotFoundException("Vehicle not found.");

            if (vehicle.Status is VehicleStatus.Rented or VehicleStatus.Reserved)
                throw new BadRequestException("A rented or reserved vehicle cannot be deleted.");

            vehicle.IsActive = false;
            await _vehicles.SaveChangesAsync();
        }

        // ---------- Images ----------

        public async Task<VehicleImageDto> AddImageAsync(int vehicleId, AddVehicleImageDto dto)
        {
            var vehicle = await _vehicles.GetByIdWithDetailsAsync(vehicleId)
                          ?? throw new NotFoundException("Vehicle not found.");

            var image = new VehicleImage
            {
                VehicleId = vehicle.Id,
                ImageUrl = dto.ImageUrl.Trim(),
                IsPrimary = dto.IsPrimary || !vehicle.Images.Any()   // first image is always primary
            };

            if (image.IsPrimary)
                foreach (var existing in vehicle.Images) existing.IsPrimary = false;

            await _vehicles.AddImageAsync(image);
            await _vehicles.SaveChangesAsync();

            return _mapper.Map<VehicleImageDto>(image);
        }

        public async Task SetPrimaryImageAsync(int vehicleId, int imageId)
        {
            var vehicle = await _vehicles.GetByIdWithDetailsAsync(vehicleId)
                          ?? throw new NotFoundException("Vehicle not found.");

            var target = vehicle.Images.FirstOrDefault(i => i.Id == imageId)
                         ?? throw new NotFoundException("Image not found.");

            foreach (var img in vehicle.Images) img.IsPrimary = img.Id == target.Id;
            await _vehicles.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int vehicleId, int imageId)
        {
            var vehicle = await _vehicles.GetByIdWithDetailsAsync(vehicleId)
                          ?? throw new NotFoundException("Vehicle not found.");

            var image = vehicle.Images.FirstOrDefault(i => i.Id == imageId)
                        ?? throw new NotFoundException("Image not found.");

            _vehicles.DeleteImage(image);

            if (image.IsPrimary)
            {
                var next = vehicle.Images.FirstOrDefault(i => i.Id != imageId);
                if (next is not null) next.IsPrimary = true;
            }

            await _vehicles.SaveChangesAsync();
        }

        // ---------- Documents ----------

        public async Task<VehicleDocumentDto> AddDocumentAsync(int vehicleId, SaveVehicleDocumentDto dto)
        {
            _ = await _vehicles.GetByIdAsync(vehicleId) ?? throw new NotFoundException("Vehicle not found.");
            ValidateDocumentDates(dto);

            var document = _mapper.Map<VehicleDocument>(dto);
            document.VehicleId = vehicleId;

            await _vehicles.AddDocumentAsync(document);
            await _vehicles.SaveChangesAsync();

            return _mapper.Map<VehicleDocumentDto>(document);
        }

        public async Task<VehicleDocumentDto> UpdateDocumentAsync(int vehicleId, int documentId, SaveVehicleDocumentDto dto)
        {
            var document = await GetDocumentAsync(vehicleId, documentId);
            ValidateDocumentDates(dto);

            _mapper.Map(dto, document);
            await _vehicles.SaveChangesAsync();

            return _mapper.Map<VehicleDocumentDto>(document);
        }

        public async Task DeleteDocumentAsync(int vehicleId, int documentId)
        {
            var document = await GetDocumentAsync(vehicleId, documentId);
            _vehicles.DeleteDocument(document);
            await _vehicles.SaveChangesAsync();
        }

        // Includes documents that have already expired
        public async Task<IEnumerable<ExpiringDocumentDto>> GetExpiringDocumentsAsync(int withinDays = 30)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var docs = await _vehicles.GetDocumentsExpiringBeforeAsync(today.AddDays(withinDays));

            return docs.Select(d => new ExpiringDocumentDto
            {
                DocumentId = d.Id,
                VehicleId = d.VehicleId,
                RegistrationNumber = d.Vehicle.RegistrationNumber,
                DocumentType = d.DocumentType,
                DocumentNumber = d.DocumentNumber,
                ExpiryDate = d.ExpiryDate,
                DaysToExpiry = d.ExpiryDate.DayNumber - today.DayNumber
            });
        }

        // ---------- Helpers ----------

        private static string NormalizeRegNo(string value) =>
            string.Concat(value.Where(c => !char.IsWhiteSpace(c))).ToUpperInvariant();

        private static void ValidateDocumentDates(SaveVehicleDocumentDto dto)
        {
            if (dto.ExpiryDate <= dto.IssueDate)
                throw new BadRequestException("Expiry date must be after the issue date.");
        }

        private async Task EnsureCategoryAndBranchAsync(SaveVehicleDto dto)
        {
            var category = await _categories.GetByIdAsync(dto.CategoryId);
            if (category is null || !category.IsActive)
                throw new BadRequestException("Selected category does not exist or is inactive.");

            var branch = await _branches.GetByIdAsync(dto.BranchId);
            if (branch is null || !branch.IsActive)
                throw new BadRequestException("Selected branch does not exist or is inactive.");
        }

        private async Task<VehicleDocument> GetDocumentAsync(int vehicleId, int documentId)
        {
            var document = await _vehicles.GetDocumentByIdAsync(documentId);
            if (document is null || document.VehicleId != vehicleId)
                throw new NotFoundException("Document not found.");
            return document;
        }
    }
}
