using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IOfferService
{
    Task<IEnumerable<OfferDto>> GetAllAsync();
    Task<OfferDto?> GetByIdAsync(int id);
    Task<IEnumerable<OfferDto>> GetActiveOffersAsync();
    Task<OfferDto> CreateAsync(CreateOfferDto dto);
    Task<OfferDto> UpdateAsync(int id, CreateOfferDto dto);
    Task DeleteAsync(int id);
}