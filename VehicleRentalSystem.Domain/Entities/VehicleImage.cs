using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class VehicleImage : BaseEntity
    {
        public int VehicleId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }
}
