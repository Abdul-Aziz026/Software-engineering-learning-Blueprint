namespace Encaptulation.Project;

public sealed class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
