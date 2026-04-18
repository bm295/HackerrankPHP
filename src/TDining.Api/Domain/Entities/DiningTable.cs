namespace TDining.Api.Domain.Entities;

public enum TableStatus
{
    Available,
    Reserved,
    Occupied,
    Cleaning
}

public sealed class DiningTable
{
    private DiningTable()
    {
    }

    public DiningTable(string code, int seats)
    {
        Code = code;
        Seats = seats;
    }

    public string Code { get; private set; } = string.Empty;
    public int Seats { get; private set; }
    public TableStatus Status { get; private set; } = TableStatus.Available;

    public void UpdateStatus(TableStatus status) => Status = status;
}
