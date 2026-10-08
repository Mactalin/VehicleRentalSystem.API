using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Vehicle
{
    public class SaveVehicleDocumentDto
    {
        public DocumentType DocumentType { get; set; }

        [Required, StringLength(100)]
        public string DocumentNumber { get; set; } = string.Empty;

        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }

        [StringLength(500)]
        public string? FileUrl { get; set; }
    }
}
