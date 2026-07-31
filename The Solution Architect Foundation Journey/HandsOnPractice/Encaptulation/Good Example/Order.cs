
namespace Encaptulation;

// ✅ Read-only view out, guarded mutation in.
public sealed class Order
{
    private const int MaxLines = 100;
    private readonly List<OrderLine> _lines = new();

    public IReadOnlyList<OrderLine> Lines => _lines;

    public void AddLine(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (_lines.Count >= MaxLines)
            throw new InvalidOperationException($"An order cannot exceed {MaxLines} lines.");
        _lines.Add(line);
    }
    // NOTE: OrderLine is a record with init-only members, so the elements are
    // immutable too. If they were mutable, IReadOnlyList would still leak them.
}



public sealed record OrderLine(string Sku, int Quantity);