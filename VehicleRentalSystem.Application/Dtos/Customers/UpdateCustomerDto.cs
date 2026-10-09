using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Customers
{
    public class UpdateCustomerDto
    {
        [Required, StringLength(150)] public string FullName { get; set; } = string.Empty;
        [Required, StringLength(20)] public string Phone { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        [Required, StringLength(300)] public string Address { get; set; } = string.Empty;
        [Required, StringLength(100)] public string City { get; set; } = string.Empty;
        [Required, StringLength(100)] public string State { get; set; } = string.Empty;
        [Required, StringLength(20)] public string PostalCode { get; set; } = string.Empty;
    }
}
