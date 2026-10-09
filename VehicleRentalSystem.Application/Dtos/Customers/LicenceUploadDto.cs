using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Customers
{
    public class LicenceUploadDto
    {
        [Required, StringLength(30)] public string LicenceNumber { get; set; } = string.Empty;
        [StringLength(150)] public string IssuingAuthority { get; set; } = string.Empty;
        [StringLength(100)] public string VehicleClasses { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        [Required, StringLength(500)] public string FilePath { get; set; } = string.Empty;
    }
}
