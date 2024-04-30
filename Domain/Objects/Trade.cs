using System;
using Domain.Types;

namespace Domain.Objects;

public sealed record Trade(string SecurityType, TransactionType TransactionType, double Quantity, double Price)
{
    public Guid Id { get; init; } = Guid.NewGuid();
}