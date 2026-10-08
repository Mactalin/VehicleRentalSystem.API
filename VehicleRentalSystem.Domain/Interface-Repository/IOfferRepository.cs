using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.RepositoryInterfaces;

public interface IOfferRepository : IGenericRepository<Offer>
{
    Task<IEnumerable<Offer>> GetActiveOffersAsync();
}
