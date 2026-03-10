namespace StatelessHttpDemo.Domain.Entities;

public enum ReservationStatus
{
    Confirmed,
    Seated,
    Cancelled,
    Completed
}

public sealed record Reservation(Guid Id, string CustomerName, string PhoneNumber, int GuestCount, DateTime BookingTimeUtc, ReservationStatus Status, string? Note);
