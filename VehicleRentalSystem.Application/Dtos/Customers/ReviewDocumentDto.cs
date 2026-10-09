using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Customers
{

    public class ReviewDocumentDto
    {
        public bool Approve { get; set; }
        [StringLength(500)] public string? RejectionReason { get; set; }   // required when Approve = false
    }
}
