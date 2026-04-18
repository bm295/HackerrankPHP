using TDining.Api.Application.DTOs;
using TDining.Api.Application.Ports.In;
using TDining.Api.Application.Ports.Out;
using TDining.Api.Domain.Entities;

namespace TDining.Api.Application.UseCases;

public sealed class ReservationUseCases(
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork) : IReservationUseCases
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
        await unitOfWork.SaveChangesAsync(cancellationToken);
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
