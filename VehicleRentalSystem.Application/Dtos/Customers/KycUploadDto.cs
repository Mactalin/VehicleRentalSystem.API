using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Customers
{

    public class KycUploadDto
    {
        public KycDocumentType DocumentType { get; set; }
        [Required, StringLength(50)] public string DocumentNumber { get; set; } = string.Empty;
        [Required, StringLength(500)] public string FilePath { get; set; } = string.Empty;   // set after file upload
    }
}
