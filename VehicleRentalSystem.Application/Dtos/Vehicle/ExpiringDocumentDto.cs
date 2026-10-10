using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Vehicle
{
    public class ExpiringDocumentDto
    {
        public int DocumentId { get; set; }
        public int VehicleId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateOnly ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
    }
}
