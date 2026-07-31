
namespace Encaptulation;

// The most common real-world leak: an escaped mutable collection

// ❌ Lines is a live handle to internal state. Caller can .Clear() / .Add() and
// bypass every rule the Order thinks it owns.
public class LeakyOrder
{
    private readonly List<OrderLine> _lines = new();
    public List<OrderLine> Lines => _lines;
}
