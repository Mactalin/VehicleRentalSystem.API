using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRentalSystem.Domain.Entities;

public class BookingAdditionalService
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int AdditionalServiceId { get; set; }
    public AdditionalService AdditionalService { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}
