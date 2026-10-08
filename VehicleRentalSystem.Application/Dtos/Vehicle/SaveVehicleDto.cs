using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Vehicle
{
    public class SaveVehicleDto
    {
        [Required, StringLength(20)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue)]
        public int BranchId { get; set; }

        [Required, StringLength(100)]
        public string Make { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Range(1990, 2100)]
        public int Year { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        public FuelType FuelType { get; set; }
        public TransmissionType Transmission { get; set; }

        [Range(1, 60)]
        public int SeatingCapacity { get; set; }

        [Range(0, int.MaxValue)]
        public int OdometerKm { get; set; }
    }
}
