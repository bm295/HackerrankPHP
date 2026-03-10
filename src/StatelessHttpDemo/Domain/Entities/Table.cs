namespace StatelessHttpDemo.Domain.Entities;

public enum TableStatus
{
    Available,
    Reserved,
    Occupied,
    Cleaning
}

public sealed class DiningTable(string code, int seats)
{
    public string Code { get; } = code;
    public int Seats { get; } = seats;
    public TableStatus Status { get; private set; } = TableStatus.Available;

    public void UpdateStatus(TableStatus status) => Status = status;
}
