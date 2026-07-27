namespace Encaptulation;

public sealed class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
