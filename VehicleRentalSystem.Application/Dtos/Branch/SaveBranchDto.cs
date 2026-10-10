using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Branch
{
    public class SaveBranchDto
    {
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(300)] public string Address { get; set; } = string.Empty;
        [Required, StringLength(100)] public string City { get; set; } = string.Empty;
        [Required, StringLength(100)] public string State { get; set; } = string.Empty;
        [Required, StringLength(10)] public string Pincode { get; set; } = string.Empty;
        [Range(-90, 90)] public double? Latitude { get; set; }
        [Range(-180, 180)] public double? Longitude { get; set; }
        [Phone, StringLength(20)] public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
