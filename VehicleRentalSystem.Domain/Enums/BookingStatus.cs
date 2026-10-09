using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Domain.Enums
{
    public enum BookingStatus
    {
        Pending = 1,     // created, awaiting advance payment
        Confirmed = 2,   // advance paid, vehicle held
        PickedUp = 3,    // rental active
        Returned = 4,    // vehicle back, charges calculated
        Completed = 5,   // invoice settled, deposit settled
        Cancelled = 6,
        Expired = 7      // advance not paid in time
    }
}
