using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Constants
{
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Staff = "Staff";
        public const string Customer = "Customer";

        public static readonly string[] All = { Admin, Manager, Staff, Customer };
    }
}
