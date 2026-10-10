using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;   // Admin, Manager, Staff, Customer

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
