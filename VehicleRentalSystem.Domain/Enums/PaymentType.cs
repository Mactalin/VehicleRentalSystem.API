using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Domain.Enums
{
    public enum PaymentType 
    {  
        Advance = 1,
        Full = 2,
        SecurityDeposit = 3,
        LateFee = 4,
        DamageCharge = 5 
    }
}
