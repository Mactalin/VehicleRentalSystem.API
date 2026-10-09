namespace VehicleRentalSystem.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object key)
        : base($"{entity} with id '{key}' was not found.") { }
}

public class BookingConflictException : DomainException
{
    public BookingConflictException()
        : base("The vehicle is not available for the selected dates.") { }
}