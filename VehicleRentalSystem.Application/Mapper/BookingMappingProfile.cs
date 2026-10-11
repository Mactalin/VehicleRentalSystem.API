using AutoMapper;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Dtos.Search;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Mapper;

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

        CreateMap<Vehicle, VehicleSearchResultDto>()
            .ForMember(d => d.VehicleId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.Name))
            .ForMember(d => d.BranchCity, o => o.MapFrom(s => s.Branch.City))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s =>
                s.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.HourlyRate, o => o.MapFrom(s => s.Category.Pricing != null ? s.Category.Pricing.HourlyPrice : 0m))
            .ForMember(d => d.DailyRate, o => o.MapFrom(s => s.Category.Pricing != null ? s.Category.Pricing.DailyPrice : 0m))
            .ForMember(d => d.WeeklyRate, o => o.MapFrom(s => s.Category.Pricing != null ? s.Category.Pricing.WeeklyPrice : 0m))
            .ForMember(d => d.SecurityDeposit, o => o.MapFrom(s => s.Category.Pricing != null ? s.Category.Pricing.SecurityDeposit : 0m))
            .ForMember(d => d.SeasonalMultiplier, o => o.Ignore())
            .ForMember(d => d.Quote, o => o.Ignore());
    }
}