using Domain.Types;

namespace Domain.Objects;

// ToDo: The problem description mentions that a trade will contain a timestamp. It should be added
public sealed record Trade(string SecurityType, TransactionType TransactionType, double Quantity, double Price)
{
    public Guid Id { get; init; } = Guid.NewGuid();
}