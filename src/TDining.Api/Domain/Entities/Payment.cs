namespace TDining.Api.Domain.Entities;

public enum PaymentMethod
{
    Cash,
    Card,
    EWallet
}

public sealed class Payment
{
    private Payment()
    {
    }

    public Payment(Guid id, Guid orderId, decimal amountVnd, PaymentMethod method, DateTime paidAtUtc)
    {
        Id = id;
        OrderId = orderId;
        AmountVnd = amountVnd;
        Method = method;
        PaidAtUtc = paidAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal AmountVnd { get; private set; }
    public PaymentMethod Method { get; private set; }
    public DateTime PaidAtUtc { get; private set; }
}
