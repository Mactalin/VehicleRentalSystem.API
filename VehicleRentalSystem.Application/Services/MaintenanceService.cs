using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Maintenance;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenance;
        private readonly IVehicleRepository _vehicles;
        private readonly IMapper _mapper;

        public MaintenanceService(IMaintenanceRepository maintenance, IVehicleRepository vehicles, IMapper mapper)
        {
            _maintenance = maintenance;
            _vehicles = vehicles;
            _mapper = mapper;
        }

        // ---------- Queries ----------

        public async Task<MaintenanceDto> GetByIdAsync(int id)
        {
            var record = await _maintenance.GetByIdWithVehicleAsync(id)
                         ?? throw new NotFoundException("Maintenance record not found.");
            return _mapper.Map<MaintenanceDto>(record);
        }

        public async Task<IEnumerable<MaintenanceDto>> GetByVehicleAsync(int vehicleId)
        {
            var vehicle = await _vehicles.GetByIdAsync(vehicleId) ?? throw new NotFoundException("Vehicle not found.");
            var dtos = _mapper.Map<List<MaintenanceDto>>(await _maintenance.GetByVehicleAsync(vehicleId));
            dtos.ForEach(d => d.RegistrationNumber = vehicle.RegistrationNumber);
            return dtos;
        }

        public async Task<IEnumerable<MaintenanceDto>> GetByStatusAsync(MaintenanceStatus status) =>
            _mapper.Map<IEnumerable<MaintenanceDto>>(await _maintenance.GetByStatusAsync(status));

        public async Task<IEnumerable<MaintenanceDto>> GetUpcomingAsync(int withinDays = 30)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return _mapper.Map<IEnumerable<MaintenanceDto>>(
                await _maintenance.GetUpcomingAsync(today, today.AddDays(withinDays)));
        }

        public async Task<decimal> GetTotalCostAsync(int vehicleId)
        {
            _ = await _vehicles.GetByIdAsync(vehicleId) ?? throw new NotFoundException("Vehicle not found.");
            return await _maintenance.GetTotalCostByVehicleAsync(vehicleId);
        }

        // ---------- Commands ----------

        public async Task<MaintenanceDto> CreateAsync(SaveMaintenanceDto dto)
        {
            _ = await _vehicles.GetByIdAsync(dto.VehicleId) ?? throw new NotFoundException("Vehicle not found.");

            var record = _mapper.Map<MaintenanceRecord>(dto);
            record.Status = MaintenanceStatus.Scheduled;

            await _maintenance.AddAsync(record);
            await _maintenance.SaveChangesAsync();

            return await GetByIdAsync(record.Id);
        }

        public async Task<MaintenanceDto> UpdateAsync(int id, SaveMaintenanceDto dto)
        {
            var record = await _maintenance.GetByIdWithVehicleAsync(id)
                         ?? throw new NotFoundException("Maintenance record not found.");

            if (record.Status != MaintenanceStatus.Scheduled)
                throw new BadRequestException("Only scheduled records can be edited.");

            if (dto.VehicleId != record.VehicleId)
                throw new BadRequestException("The vehicle of a maintenance record cannot be changed.");

            _mapper.Map(dto, record);
            await _maintenance.SaveChangesAsync();

            return _mapper.Map<MaintenanceDto>(record);
        }

        public async Task<MaintenanceDto> UpdateStatusAsync(int id, MaintenanceStatus status)
        {
            var record = await _maintenance.GetByIdWithVehicleAsync(id)
                         ?? throw new NotFoundException("Maintenance record not found.");

            if (record.Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
                throw new BadRequestException("A finished record cannot change status.");

            switch (status)
            {
                case MaintenanceStatus.InProgress:
                    if (record.Status != MaintenanceStatus.Scheduled)
                        throw new BadRequestException("Only scheduled records can be started.");

                    if (record.Vehicle.Status is VehicleStatus.Rented or VehicleStatus.Reserved)
                        throw new BadRequestException("This vehicle is currently rented or reserved.");

                    record.Vehicle.Status = VehicleStatus.Maintenance;
                    break;

                case MaintenanceStatus.Cancelled:
                    if (record.Status == MaintenanceStatus.InProgress)
                        await ReleaseVehicleAsync(record);
                    break;

                case MaintenanceStatus.Completed:
                    throw new BadRequestException("Use the complete endpoint to finish a record (it captures the final cost).");

                default:
                    throw new BadRequestException("Invalid status change.");
            }

            record.Status = status;
            await _maintenance.SaveChangesAsync();

            return _mapper.Map<MaintenanceDto>(record);
        }

        public async Task<MaintenanceDto> CompleteAsync(int id, CompleteMaintenanceDto dto)
        {
            var record = await _maintenance.GetByIdWithVehicleAsync(id)
                         ?? throw new NotFoundException("Maintenance record not found.");

            if (record.Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
                throw new BadRequestException("This record is already finished.");

            var wasInProgress = record.Status == MaintenanceStatus.InProgress;

            record.Status = MaintenanceStatus.Completed;
            record.Cost = dto.Cost;
            record.CompletedDate = dto.CompletedDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            if (wasInProgress)
                await ReleaseVehicleAsync(record);

            await _maintenance.SaveChangesAsync();
            return _mapper.Map<MaintenanceDto>(record);
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _maintenance.GetByIdAsync(id)
                         ?? throw new NotFoundException("Maintenance record not found.");

            if (record.Status == MaintenanceStatus.InProgress)
                throw new BadRequestException("Cancel or complete the record before deleting it.");

            _maintenance.Delete(record);
            await _maintenance.SaveChangesAsync();
        }

        // Puts the vehicle back to Available unless another maintenance job is still running on it
        private async Task ReleaseVehicleAsync(MaintenanceRecord record)
        {
            var all = await _maintenance.GetByVehicleAsync(record.VehicleId);
            var otherActive = all.Any(m => m.Id != record.Id && m.Status == MaintenanceStatus.InProgress);

            if (!otherActive && record.Vehicle.Status == VehicleStatus.Maintenance)
                record.Vehicle.Status = VehicleStatus.Available;
        }
    }
}
