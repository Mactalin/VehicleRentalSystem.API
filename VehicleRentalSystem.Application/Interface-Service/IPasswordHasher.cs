using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
