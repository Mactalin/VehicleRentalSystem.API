using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Branch;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branches;
        private readonly IVehicleRepository _vehicles;
        private readonly IMapper _mapper;

        public BranchService(IBranchRepository branches, IVehicleRepository vehicles, IMapper mapper)
        {
            _branches = branches;
            _vehicles = vehicles;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BranchDto>> GetAllAsync(string? city = null)
        {
            var list = string.IsNullOrWhiteSpace(city)
                ? (await _branches.GetAllWithHoursAsync()).Where(b => b.IsActive)
                : await _branches.GetByCityAsync(city.Trim());

            return _mapper.Map<IEnumerable<BranchDto>>(list);
        }

        public async Task<BranchDto> GetByIdAsync(int id)
        {
            var branch = await _branches.GetByIdWithHoursAsync(id) ?? throw new NotFoundException("Branch not found.");
            return _mapper.Map<BranchDto>(branch);
        }

        public async Task<BranchDto> CreateAsync(SaveBranchDto dto)
        {
            var branch = _mapper.Map<Branch>(dto);
            await _branches.AddAsync(branch);
            await _branches.SaveChangesAsync();
            return _mapper.Map<BranchDto>(branch);
        }

        public async Task<BranchDto> UpdateAsync(int id, SaveBranchDto dto)
        {
            var branch = await _branches.GetByIdAsync(id) ?? throw new NotFoundException("Branch not found.");

            if (!dto.IsActive && await HasActiveVehiclesAsync(id))
                throw new BadRequestException("Move or deactivate this branch's vehicles before deactivating it.");

            _mapper.Map(dto, branch);
            await _branches.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var branch = await _branches.GetByIdAsync(id) ?? throw new NotFoundException("Branch not found.");

            if (await HasActiveVehiclesAsync(id))
                throw new BadRequestException("This branch still has vehicles assigned. Reassign them first.");

            branch.IsActive = false;   // soft delete
            await _branches.SaveChangesAsync();
        }

        public async Task<IEnumerable<OperatingHourDto>> GetOperatingHoursAsync(int branchId)
        {
            _ = await _branches.GetByIdAsync(branchId) ?? throw new NotFoundException("Branch not found.");
            var hours = await _branches.GetOperatingHoursAsync(branchId);
            return _mapper.Map<IEnumerable<OperatingHourDto>>(
                hours.OrderBy(h => ((int)h.DayOfWeek + 6) % 7));
        }

        // Replaces the weekly schedule: updates existing days in place and adds missing ones
        public async Task<IEnumerable<OperatingHourDto>> SetOperatingHoursAsync(
            int branchId, IEnumerable<OperatingHourDto> hours)
        {
            _ = await _branches.GetByIdAsync(branchId) ?? throw new NotFoundException("Branch not found.");

            var incoming = hours.ToList();

            if (incoming.GroupBy(h => h.DayOfWeek).Any(g => g.Count() > 1))
                throw new BadRequestException("Each day of the week can appear only once.");

            if (incoming.Any(h => !h.IsClosed && h.CloseTime <= h.OpenTime))
                throw new BadRequestException("Closing time must be after opening time.");

            var existing = (await _branches.GetOperatingHoursAsync(branchId)).ToDictionary(h => h.DayOfWeek);
            var toAdd = new List<BranchOperatingHour>();

            foreach (var dto in incoming)
            {
                if (existing.TryGetValue(dto.DayOfWeek, out var row))
                {
                    row.OpenTime = dto.OpenTime;
                    row.CloseTime = dto.CloseTime;
                    row.IsClosed = dto.IsClosed;
                }
                else
                {
                    var newRow = _mapper.Map<BranchOperatingHour>(dto);
                    newRow.BranchId = branchId;
                    toAdd.Add(newRow);
                }
            }

            if (toAdd.Count > 0) await _branches.AddOperatingHoursAsync(toAdd);
            await _branches.SaveChangesAsync();

            return await GetOperatingHoursAsync(branchId);
        }

        private async Task<bool> HasActiveVehiclesAsync(int branchId) =>
            await _vehicles.ExistsAsync(v => v.BranchId == branchId && v.IsActive);
    }
}
