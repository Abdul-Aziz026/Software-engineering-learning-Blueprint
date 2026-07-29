
namespace Abstraction;

public readonly record struct Money(decimal Amount, string Currency)
{
    public override string ToString() => $"{Amount:0.00} {Currency}";
}
