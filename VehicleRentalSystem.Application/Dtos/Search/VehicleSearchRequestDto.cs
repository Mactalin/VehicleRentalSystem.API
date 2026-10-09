using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Search
{
    public class VehicleSearchRequestDto
    {
        public int? BranchId { get; set; }
        public string? City { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<int>? FeatureIds { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
