using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public int? VehicleId { get; set; }
    public int? DriverId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public int? VehicleId { get; set; }
    public int? DriverId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}