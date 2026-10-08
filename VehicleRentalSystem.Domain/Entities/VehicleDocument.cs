using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class VehicleDocument : BaseEntity
    {
        public int VehicleId { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string? FileUrl { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }
}
