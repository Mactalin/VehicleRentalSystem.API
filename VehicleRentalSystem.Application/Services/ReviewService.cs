using AutoMapper;
using VehicleRentalSystem.Application.DTOs.Reviews;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repo;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repo.GetAllAsync(ct));

    public async Task<ReviewDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var r = await _repo.GetByIdAsync(id, ct);
        return r is null ? null : _mapper.Map<ReviewDto>(r);
    }

    public async Task<IEnumerable<ReviewDto>> GetByVehicleIdAsync(int vehicleId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repo.GetByVehicleIdAsync(vehicleId, ct));

    public async Task<IEnumerable<ReviewDto>> GetByDriverIdAsync(int driverId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repo.GetByDriverIdAsync(driverId, ct));

    public async Task<IEnumerable<ReviewDto>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repo.GetByCustomerIdAsync(customerId, ct));

    public async Task<ReviewSummaryDto> GetVehicleSummaryAsync(int vehicleId, CancellationToken ct = default)
    {
        var reviews = (await _repo.GetByVehicleIdAsync(vehicleId, ct)).ToList();
        return BuildSummary(reviews);
    }

    public async Task<ReviewSummaryDto> GetDriverSummaryAsync(int driverId, CancellationToken ct = default)
    {
        var reviews = (await _repo.GetByDriverIdAsync(driverId, ct)).ToList();
        return BuildSummary(reviews);
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto, CancellationToken ct = default)
    {
        if (await _repo.HasReviewedBookingAsync(dto.BookingId, ct))
            throw new DomainException("This booking has already been reviewed.");

        var review = _mapper.Map<Review>(dto);
        review.ValidateRating();
        review.PostedAt = DateTime.UtcNow;
        review.IsVerifiedBooking = true;

        await _repo.AddAsync(review, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<ReviewDto>(review);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var r = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Review), id);
        _repo.Delete(r);
        await _repo.SaveChangesAsync(ct);
    }

    private static ReviewSummaryDto BuildSummary(List<Review> reviews)
    {
        if (reviews.Count == 0)
            return new ReviewSummaryDto();

        return new ReviewSummaryDto
        {
            TotalReviews = reviews.Count,
            AverageRating = Math.Round(reviews.Average(r => r.Rating), 2),
            FiveStarCount = reviews.Count(r => r.Rating == 5),
            FourStarCount = reviews.Count(r => r.Rating == 4),
            ThreeStarCount = reviews.Count(r => r.Rating == 3),
            TwoStarCount = reviews.Count(r => r.Rating == 2),
            OneStarCount = reviews.Count(r => r.Rating == 1)
        };
    }
}