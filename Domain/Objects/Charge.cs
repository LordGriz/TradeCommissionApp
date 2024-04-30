namespace Domain.Objects;

public sealed record Charge(Guid TradeId, double Commission)
{
    public Guid Id { get; init; } = Guid.NewGuid();
}