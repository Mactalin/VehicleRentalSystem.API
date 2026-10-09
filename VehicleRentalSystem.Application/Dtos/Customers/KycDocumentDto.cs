using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Customers
{

    public class KycDocumentDto
    {
        public int Id { get; set; }
        public KycDocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public VerificationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
