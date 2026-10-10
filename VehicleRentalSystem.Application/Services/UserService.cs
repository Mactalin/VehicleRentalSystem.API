using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Constants;
using VehicleRentalSystem.Application.Dtos.User;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IMapper _mapper;

        public UserService(IUserRepository users, IPasswordHasher hasher, IMapper mapper)
        {
            _users = users;
            _hasher = hasher;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<UserDto>>(await _users.GetAllWithRolesAsync());

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _users.GetByIdAsync(id) ?? throw new NotFoundException("User not found.");
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _users.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");

            user.FullName = dto.FullName.Trim();
            user.Phone = dto.Phone?.Trim();
            await _users.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            if (await _users.EmailExistsAsync(email))
                throw new ConflictException("This email is already registered.");

            var role = await ResolveRoleAsync(dto.Role);

            var user = new User
            {
                Role = role,
                RoleId = role.Id,
                FullName = dto.FullName.Trim(),
                Email = email,
                Phone = dto.Phone?.Trim(),
                PasswordHash = _hasher.Hash(dto.Password),
                IsActive = true
            };

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateRoleAsync(int id, UpdateUserRoleDto dto, int currentUserId)
        {
            if (id == currentUserId)
                throw new BadRequestException("You cannot change your own role.");

            var user = await _users.GetByIdAsync(id) ?? throw new NotFoundException("User not found.");
            var role = await ResolveRoleAsync(dto.Role);

            user.Role = role;
            user.RoleId = role.Id;
            await _users.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task SetActiveAsync(int id, bool isActive, int currentUserId)
        {
            if (id == currentUserId && !isActive)
                throw new BadRequestException("You cannot deactivate your own account.");

            var user = await _users.GetByIdAsync(id) ?? throw new NotFoundException("User not found.");
            user.IsActive = isActive;
            await _users.SaveChangesAsync();
        }

        public Task<IEnumerable<string>> GetRolesAsync() =>
            Task.FromResult<IEnumerable<string>>(RoleNames.All);

        private async Task<Role> ResolveRoleAsync(string roleName)
        {
            var normalized = RoleNames.All.FirstOrDefault(r =>
                string.Equals(r, roleName.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? throw new BadRequestException($"Invalid role. Allowed: {string.Join(", ", RoleNames.All)}.");

            return await _users.GetRoleByNameAsync(normalized)
                   ?? throw new NotFoundException($"Role '{normalized}' is not configured.");
        }
    }
}
