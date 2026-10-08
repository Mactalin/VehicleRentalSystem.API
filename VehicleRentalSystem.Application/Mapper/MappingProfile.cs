using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Domain.Entities;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VehicleRentalSystem.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Driver, DriverDto>().ReverseMap();
        CreateMap<CreateDriverDto, Driver>();
        CreateMap<UpdateDriverDto, Driver>();

        CreateMap<AdditionalService, AdditionalServiceDto>().ReverseMap();
        CreateMap<CreateAdditionalServiceDto, AdditionalService>();

        CreateMap<Coupon, CouponDto>().ReverseMap();
        CreateMap<CreateCouponDto, Coupon>();

        CreateMap<Offer, OfferDto>().ReverseMap();
        CreateMap<CreateOfferDto, Offer>();

        CreateMap<Review, ReviewDto>().ReverseMap();
        CreateMap<CreateReviewDto, Review>();

        CreateMap<Complaint, ComplaintDto>().ReverseMap();
        CreateMap<CreateComplaintDto, Complaint>();

        CreateMap<Notification, NotificationDto>().ReverseMap();
        CreateMap<CreateNotificationDto, Notification>();
    }
}