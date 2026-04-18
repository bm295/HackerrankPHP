namespace TDining.Api.Domain.Entities;

public enum ReservationStatus
{
    Confirmed,
    Seated,
    Cancelled,
    Completed
}

public sealed class Reservation
{
    private Reservation()
    {
    }

    public Reservation(Guid id, string customerName, string phoneNumber, int guestCount, DateTime bookingTimeUtc, ReservationStatus status, string? note)
    {
        Id = id;
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        GuestCount = guestCount;
        BookingTimeUtc = bookingTimeUtc;
        Status = status;
        Note = note;
    }

    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public int GuestCount { get; private set; }
    public DateTime BookingTimeUtc { get; private set; }
    public ReservationStatus Status { get; private set; }
    public string? Note { get; private set; }
}
