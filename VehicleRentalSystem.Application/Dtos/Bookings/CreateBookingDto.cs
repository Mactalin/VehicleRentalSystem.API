using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{
    public class CreateBookingDto
    {
        public int CustomerId { get; set; }           // controller overwrites from JWT for customers
        [Range(1, int.MaxValue)] public int VehicleId { get; set; }
        public int? PickupBranchId { get; set; }      // defaults to the vehicle's branch
        public int? DropBranchId { get; set; }        // defaults to the pickup branch
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
