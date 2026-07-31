// Level up — make illegal states unrepresentable
namespace Encaptulation.Project;


// `decimal` allows -5000, so BankAccount needs a runtime check.
// A value object pushes the error out of BankAccount entirely.
public readonly record struct Money
{
    public decimal Value { get; }
    private Money(decimal value) => Value = value;

    public static Money Positive(decimal value) =>
        value > 0
            ? new Money(value)
            : throw new ArgumentOutOfRangeException(nameof(value), "Money must be positive.");
    
    // "N2" = 2 decimal places, thousands separator example: 1,234.5634 becomes 1,234.56
    public override string ToString() => Value.ToString("N2"); 
}

