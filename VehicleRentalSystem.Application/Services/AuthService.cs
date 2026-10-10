using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Constants;
using VehicleRentalSystem.Application.Dtos.Auth;
using VehicleRentalSystem.Application.Dtos.User;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenService _jwt;
        private readonly IPasswordHasher _hasher;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository users, IJwtTokenService jwt, IPasswordHasher hasher, IMapper mapper)
        {
            _users = users;
            _jwt = jwt;
            _hasher = hasher;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _users.EmailExistsAsync(email))
                throw new ConflictException("This email is already registered.");

            // Public registration always creates a Customer
            var role = await _users.GetRoleByNameAsync(RoleNames.Customer)
                       ?? throw new NotFoundException("Customer role is not configured.");

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

            return new AuthResponseDto
            {
                Token = _jwt.GenerateToken(user),
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email.Trim());

            if (user is null || !_hasher.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedException("This account has been deactivated. Please contact support.");

            user.LastLoginAt = DateTime.UtcNow;
            await _users.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = _jwt.GenerateToken(user),
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _users.GetByIdAsync(userId)
                       ?? throw new NotFoundException("User not found.");

            if (!_hasher.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new BadRequestException("Current password is incorrect.");

            if (dto.CurrentPassword == dto.NewPassword)
                throw new BadRequestException("New password must be different from the current password.");

            user.PasswordHash = _hasher.Hash(dto.NewPassword);
            await _users.SaveChangesAsync();
        }
    }
}
