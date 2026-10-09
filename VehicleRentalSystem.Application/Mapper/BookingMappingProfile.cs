using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Dtos.Search;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Mapper
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer.FullName))
                .ForMember(d => d.VehicleName, o => o.MapFrom(s => s.Vehicle.Name))
                .ForMember(d => d.VehicleRegistration, o => o.MapFrom(s => s.Vehicle.RegistrationNumber));

            CreateMap<Booking, BookingSummaryDto>()
                .ForMember(d => d.VehicleName, o => o.MapFrom(s => s.Vehicle.Name));

            CreateMap<VehicleInspection, InspectionDto>();

            // Search results
            CreateMap<Vehicle, VehicleSearchResultDto>()
                .ForMember(d => d.VehicleId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Quote, o => o.Ignore());
        }
    }
}
