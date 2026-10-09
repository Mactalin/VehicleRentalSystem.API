using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Customers;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Mapper
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            CreateMap<Customer, CustomerDto>();
            CreateMap<Customer, CustomerSummaryDto>();

            CreateMap<KycUploadDto, KycDocument>();
            CreateMap<KycDocument, KycDocumentDto>();

            CreateMap<LicenceUploadDto, DrivingLicence>();
            CreateMap<DrivingLicence, DrivingLicenceDto>();
        }
    }
}
