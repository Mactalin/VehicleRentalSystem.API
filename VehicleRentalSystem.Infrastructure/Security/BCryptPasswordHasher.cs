using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Interface_Service;

namespace VehicleRentalSystem.Infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
