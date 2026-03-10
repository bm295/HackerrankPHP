namespace StatelessHttpDemo.Domain.Entities;

public enum PaymentMethod
{
    Cash,
    Card,
    EWallet
}

public sealed record Payment(Guid Id, Guid OrderId, decimal AmountVnd, PaymentMethod Method, DateTime PaidAtUtc);
