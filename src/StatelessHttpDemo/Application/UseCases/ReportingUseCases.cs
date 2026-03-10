using StatelessHttpDemo.Application.DTOs;
using StatelessHttpDemo.Application.Ports.In;
using StatelessHttpDemo.Application.Ports.Out;
using StatelessHttpDemo.Domain.Entities;

namespace StatelessHttpDemo.Application.UseCases;

public sealed class ReportingUseCases(IOrderRepository orderRepository) : IReportingUseCases
{
    public async Task<DailyReportDto> GetDailyReportAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.ListAsync(cancellationToken);
        var selected = orders.Where(o => DateOnly.FromDateTime(o.CreatedAtUtc) == date).ToList();

        return new DailyReportDto(
            date,
            selected.Count,
            selected.Sum(o => o.TotalAmountVnd),
            selected.Count(o => o.Status == OrderStatus.Closed),
            selected.Count(o => o.Status is OrderStatus.New or OrderStatus.Preparing or OrderStatus.Served));
    }
}
