using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Vehicle
{
    public class VehicleDocumentDto
    {
        public int Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string? FileUrl { get; set; }
        public int DaysToExpiry { get; set; }
        public bool IsExpired { get; set; }
    }
}
