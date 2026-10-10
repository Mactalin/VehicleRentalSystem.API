using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Exceptions
{
    public class NotFoundException(string message) : Exception(message);       // 404
    public class BadRequestException(string message) : Exception(message);     // 400
    public class ConflictException(string message) : Exception(message);       // 409
    public class UnauthorizedException(string message) : Exception(message);   // 401
}
