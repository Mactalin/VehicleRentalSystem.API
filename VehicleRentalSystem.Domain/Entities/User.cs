using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public int RoleId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }

        public Role Role { get; set; } = null!;
    }
}
