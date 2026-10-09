using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Customers
{
    public class UpdateCustomerStatusDto
    {
        public CustomerStatus Status { get; set; }
    }
}
