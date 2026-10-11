using System.Net;

namespace VehicleRentalSystem.Authorization;

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string AdminOrManager = "AdminOrManager";
        public const string StaffAndAbove = "StaffAndAbove";
    }

