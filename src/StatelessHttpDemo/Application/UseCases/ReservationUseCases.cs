using StatelessHttpDemo.Application.DTOs;
using StatelessHttpDemo.Application.Ports.In;
using StatelessHttpDemo.Application.Ports.Out;
using StatelessHttpDemo.Domain.Entities;

namespace StatelessHttpDemo.Application.UseCases;

public sealed class ReservationUseCases(IReservationRepository reservationRepository) : IReservationUseCases
{
    private const int MaxCapacity = 70;

    public async Task<ReservationDto> CreateReservationAsync(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var reservations = await reservationRepository.ListAsync(cancellationToken);
        var reservedGuests = reservations
            .Where(r => r.Status == ReservationStatus.Confirmed && r.BookingTimeUtc.Date == command.BookingTimeUtc.Date)
            .Sum(r => r.GuestCount);

        if (reservedGuests + command.GuestCount > MaxCapacity)
        {
            throw new InvalidOperationException("Reservation exceeds configured seating capacity.");
        }

        var reservation = new Reservation(Guid.NewGuid(), command.CustomerName.Trim(), command.PhoneNumber.Trim(), command.GuestCount, command.BookingTimeUtc, ReservationStatus.Confirmed, command.Note?.Trim());
        await reservationRepository.AddAsync(reservation, cancellationToken);
        return ToDto(reservation);
    }

    public async Task<IReadOnlyCollection<ReservationDto>> ListReservationsAsync(CancellationToken cancellationToken)
    {
        var reservations = await reservationRepository.ListAsync(cancellationToken);
        return reservations.Select(ToDto).ToList();
    }

    private static ReservationDto ToDto(Reservation reservation) =>
        new(reservation.Id, reservation.CustomerName, reservation.PhoneNumber, reservation.GuestCount, reservation.BookingTimeUtc, reservation.Status.ToString(), reservation.Note);
}
