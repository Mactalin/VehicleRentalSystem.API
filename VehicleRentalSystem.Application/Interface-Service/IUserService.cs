using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.User;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateRoleAsync(int id, UpdateUserRoleDto dto, int currentUserId);
        Task SetActiveAsync(int id, bool isActive, int currentUserId);
        Task<IEnumerable<string>> GetRolesAsync();
    }
}
