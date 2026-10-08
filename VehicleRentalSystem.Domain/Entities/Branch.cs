using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<BranchOperatingHour> OperatingHours { get; set; } = new List<BranchOperatingHour>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
