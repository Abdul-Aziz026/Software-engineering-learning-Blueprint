
namespace Inheritance;

public class LegacyReport
{
    public string Title { get; }

    public LegacyReport(string title) => Title = title;

    public virtual void Render() => Console.WriteLine($"[report] {Title}");

    public void Export() => Console.WriteLine($"[export] {Title}.pdf");

    public void Delete() => Console.WriteLine($"{Title} DELETED");
}

public class ReadOnlyAuditReport  : LegacyReport
{
    public ReadOnlyAuditReport(string title) : base(title) { }

    public override void Render() => Console.WriteLine($"[audit·immutable] {Title}");

    // Notice: this class says nothing about deleting.
    // It will inherit Delete() anyway. Silently.

    // wrong Inheritance...
}
