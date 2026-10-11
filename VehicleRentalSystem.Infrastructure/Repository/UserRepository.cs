using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public override async Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
    await _dbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbSet.Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Email == email.ToLower());

        public async Task<bool> EmailExistsAsync(string email) =>
            await _dbSet.AnyAsync(u => u.Email == email.ToLower());

        public async Task<IEnumerable<User>> GetAllWithRolesAsync() =>
            await _dbSet.AsNoTracking().Include(u => u.Role)
                        .OrderBy(u => u.FullName).ToListAsync();

        public async Task<Role?> GetRoleByNameAsync(string roleName) =>
            await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        public async Task<IEnumerable<Role>> GetAllRolesAsync() =>
            await _context.Roles.AsNoTracking().ToListAsync();
    }
}
