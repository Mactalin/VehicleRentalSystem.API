using AutoMapper;
using VehicleRentalSystem.Application.DTOs.Offers;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _repo;
    private readonly IMapper _mapper;

    public OfferService(IOfferRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OfferDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<OfferDto>>(await _repo.GetAllAsync(ct));

    public async Task<IEnumerable<OfferDto>> GetRunningAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<OfferDto>>(await _repo.GetRunningAsync(ct));

    public async Task<OfferDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var o = await _repo.GetByIdAsync(id, ct);
        return o is null ? null : _mapper.Map<OfferDto>(o);
    }

    public async Task<OfferDto> CreateAsync(CreateOfferDto dto, CancellationToken ct = default)
    {
        if (dto.EndDate <= dto.StartDate)
            throw new DomainException("EndDate must be after StartDate.");

        var offer = _mapper.Map<Offer>(dto);
        offer.IsActive = true;

        await _repo.AddAsync(offer, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<OfferDto>(offer);
    }

    public async Task<OfferDto> UpdateAsync(int id, UpdateOfferDto dto, CancellationToken ct = default)
    {
        var offer = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Offer), id);

        _mapper.Map(dto, offer);
        _repo.Update(offer);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<OfferDto>(offer);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var offer = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Offer), id);
        _repo.Delete(offer);
        await _repo.SaveChangesAsync(ct);
    }
}