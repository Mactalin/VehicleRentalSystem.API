using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Mapper
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<Payment, PaymentDto>();
            CreateMap<Refund, RefundDto>();

            CreateMap<InvoiceItem, InvoiceItemDto>();
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.BookingNumber, o => o.Ignore())   // filled in InvoiceService
                .ForMember(d => d.CustomerName, o => o.Ignore());
        }
    }
}
