using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
    public string Name { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int BranchId { get; set; }

        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Color { get; set; }

        public FuelType FuelType { get; set; }
        public TransmissionType Transmission { get; set; }
        public int SeatingCapacity { get; set; }
        public int OdometerKm { get; set; }

        public VehicleStatus Status { get; set; } = VehicleStatus.Available;
        public bool IsActive { get; set; } = true;
}

        public VehicleCategory Category { get; set; } = null!;
        public Branch Branch { get; set; } = null!;
        public ICollection<VehicleImage> Images { get; set; } = new List<VehicleImage>();
        public ICollection<VehicleDocument> Documents { get; set; } = new List<VehicleDocument>();
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    }
}
