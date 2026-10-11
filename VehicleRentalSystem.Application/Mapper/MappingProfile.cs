using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Branch;
using VehicleRentalSystem.Application.Dtos.Category;
using VehicleRentalSystem.Application.Dtos.Maintenance;
using VehicleRentalSystem.Application.Dtos.User;
using VehicleRentalSystem.Application.Dtos.Vehicle;
using VehicleRentalSystem.Application.DTOs.AdditionalServices;
using VehicleRentalSystem.Application.DTOs.Complaints;
using VehicleRentalSystem.Application.DTOs.Coupons;
using VehicleRentalSystem.Application.DTOs.Drivers;
using VehicleRentalSystem.Application.DTOs.Notifications;
using VehicleRentalSystem.Application.DTOs.Offers;
using VehicleRentalSystem.Application.DTOs.Reviews;
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

            //Lenin
            // Drivers
            CreateMap<Driver, DriverDto>();
            CreateMap<CreateDriverDto, Driver>();
            CreateMap<UpdateDriverDto, Driver>();

            // Additional Services
            CreateMap<AdditionalService, AdditionalServiceDto>();
            CreateMap<CreateAdditionalServiceDto, AdditionalService>();
            CreateMap<UpdateAdditionalServiceDto, AdditionalService>();

            // Coupons
            CreateMap<Coupon, CouponDto>();
            CreateMap<CreateCouponDto, Coupon>();

            // Offers
            CreateMap<Offer, OfferDto>();
            CreateMap<CreateOfferDto, Offer>();
            CreateMap<UpdateOfferDto, Offer>();

            // Reviews
            CreateMap<Review, ReviewDto>()
                .ForMember(d => d.CustomerName,
                    o => o.MapFrom(s => s.Customer != null ? s.Customer.FullName : null))
                .ForMember(d => d.DriverName,
                    o => o.MapFrom(s => s.Driver != null ? s.Driver.FullName : null));
            CreateMap<CreateReviewDto, Review>();

            // Complaints
            CreateMap<Complaint, ComplaintDto>()
                .ForMember(d => d.CustomerName,
                    o => o.MapFrom(s => s.Customer != null ? s.Customer.FullName : null));
            CreateMap<CreateComplaintDto, Complaint>();

            // Notifications
            CreateMap<Notification, NotificationDto>();
            CreateMap<CreateNotificationDto, Notification>();

        }
    }
}
