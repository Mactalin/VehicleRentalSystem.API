using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Branch;
using VehicleRentalSystem.Application.Dtos.Category;
using VehicleRentalSystem.Application.Dtos.Maintenance;
using VehicleRentalSystem.Application.Dtos.User;
using VehicleRentalSystem.Application.Dtos.Vehicle;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Users
            CreateMap<User, UserDto>()
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.Name));

            // Vehicles
            CreateMap<SaveVehicleDto, Vehicle>();
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Branch.Name))
                .ForMember(d => d.PrimaryImageUrl, o => o.MapFrom(s =>
                    s.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()));

            CreateMap<VehicleImage, VehicleImageDto>();

            CreateMap<SaveVehicleDocumentDto, VehicleDocument>();
            CreateMap<VehicleDocument, VehicleDocumentDto>()
                .ForMember(d => d.DaysToExpiry, o => o.MapFrom(s =>
                    s.ExpiryDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber))
                .ForMember(d => d.IsExpired, o => o.MapFrom(s =>
                    s.ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow)));

            // Categories & pricing
            CreateMap<VehicleCategory, CategoryDto>();
            CreateMap<SaveCategoryDto, VehicleCategory>();
            CreateMap<CategoryPricing, PricingDto>().ReverseMap();

            CreateMap<SeasonalPricing, SeasonalPricingDto>();
            CreateMap<SaveSeasonalPricingDto, SeasonalPricing>();

            CreateMap<AdditionalCharge, AdditionalChargeDto>();
            CreateMap<SaveAdditionalChargeDto, AdditionalCharge>();

            // Branches
            CreateMap<Branch, BranchDto>()
                .ForMember(d => d.OperatingHours, o => o.MapFrom(s =>
                    s.OperatingHours.OrderBy(h => ((int)h.DayOfWeek + 6) % 7)));   // Monday first
            CreateMap<SaveBranchDto, Branch>();
            CreateMap<BranchOperatingHour, OperatingHourDto>().ReverseMap();

            // Maintenance
            CreateMap<MaintenanceRecord, MaintenanceDto>()
                .ForMember(d => d.RegistrationNumber, o => o.MapFrom(s => s.Vehicle.RegistrationNumber));
            CreateMap<SaveMaintenanceDto, MaintenanceRecord>();
        }
    }
}
