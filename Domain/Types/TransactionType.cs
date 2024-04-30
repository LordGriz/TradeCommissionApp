

namespace Domain.Types;

///
/// Note: 
/// 
//public sealed record TransactionType(string Name, int Value)
//{
//    public static readonly TransactionType Buy = new("BUY", 1);
//    public static readonly TransactionType Sell = new("SELL", 2);

//    public static readonly Dictionary<int, TransactionType> _typed = new() { {1, Buy}, {2, Sell}};

//    public static TransactionType Create(int value)
//    {
//        return _typed.TryGetValue(value, out var result)
//            ? result
//            : new TransactionType("UnknownType", value);
//    }
//}

public enum TransactionType
{
    Unknown = 0,
    Buy = 1,
    Sell = 2
}
