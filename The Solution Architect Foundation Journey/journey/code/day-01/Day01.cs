// Day 01 — Encapsulation as invariant protection
// Journey: The Solution Architect Foundation Journey (Day 1 of 90)
//
// HOW TO RUN  (needs .NET 6+ / C# 10 — uses DateOnly and record struct)
//   dotnet new console -o day01
//   then replace day01/Program.cs with this file
//   cd day01 && dotnet run
//
// (dotnet-script also works but only if it targets C# 10 or later.)
//
// TASK: don't just run it. Retype BankAccount yourself, then do STEP 3 (the daily
// withdrawal cap) — that's where encapsulation stops being syntax and becomes design.

using System;
using System.Collections.Generic;

#region 1. The problem — naive versions that hurt

// ❌ private field + public setter. Looks like OOP. Encapsulation is zero.
public class NaiveBankAccount
{
    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        set => _balance = value;   // no gate. anyone can write anything.
    }
}

// ❌ Validation on Withdraw only. Deposit is the unguarded back door.
public class HalfGuardedBankAccount
{
    public decimal Balance { get; private set; }

    public void Withdraw(decimal amount)
    {
        if (Balance - amount < 0) throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }

    // Deposit(-5000) walks straight past the Withdraw guard.
    public void Deposit(decimal amount) => Balance += amount;
}

#endregion

#region 2. The invariant-protected version

/// <summary>
/// INVARIANT: Balance >= 0 at every observable moment, from construction onward.
/// Every door into this object (ctor, Deposit, Withdraw) is guarded.
/// sealed: nobody can subclass and override the rule away.
/// </summary>
public sealed class BankAccount
{
    private decimal _balance;

    public BankAccount(decimal openingBalance)          // door #1
    {
        if (openingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance),
                "Opening balance cannot be negative.");
        _balance = openingBalance;
    }

    public decimal Balance => _balance;                 // read-only outward. no setter.

    public void Deposit(decimal amount)                 // door #2
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be positive.");
        _balance += amount;
    }

    public void Withdraw(decimal amount)                // door #3
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal must be positive.");
        if (amount > _balance)
            throw new InvalidOperationException("Insufficient funds.");
        _balance -= amount;
    }
}

#endregion

#region 3. Level up — make illegal states unrepresentable (preview, Day 19)

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

    public override string ToString() => Value.ToString("N2");
}

#endregion

#region 4. The most common real-world leak: an escaped mutable collection

public sealed record OrderLine(string Sku, int Quantity);

// ❌ Lines is a live handle to internal state. Caller can .Clear() / .Add() and
//    bypass every rule the Order thinks it owns.
public class LeakyOrder
{
    private readonly List<OrderLine> _lines = new();
    public List<OrderLine> Lines => _lines;
}

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

#endregion

#region 5. YOUR TASK — daily withdrawal cap (an invariant spanning multiple doors)

// Rule: total withdrawals in a single day must not exceed 50,000.
//
// Questions to answer with your design (this is the real lesson):
//   - Where does "today's withdrawn total" live?  (Hint: inside, always inside.)
//   - Who resets it? If the caller can reset it, the caller can bypass the rule.
//   - How does the object know what "today" is without you passing it in every call
//     (which the caller could lie about)?  -> inject a clock abstraction. That's DIP,
//     Day 17. Notice how the principles start pulling on each other.
//
// Delete the NotImplementedException and write it yourself.

public interface IClock { DateOnly Today { get; } }

public sealed class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}

public sealed class CappedBankAccount
{
    // INVARIANT 1: Balance >= 0
    // INVARIANT 2: sum of withdrawals on any single day <= DailyWithdrawalLimit
    public const decimal DailyWithdrawalLimit = 50_000m;

    private readonly IClock _clock;
    private decimal _balance;
    private DateOnly _windowDate;
    private decimal _withdrawnToday;

    public CappedBankAccount(decimal openingBalance, IClock clock)
    {
        // TODO(you): guard openingBalance, guard clock null, seed the window.
        throw new NotImplementedException("Day 1 task — write this yourself.");
    }

    public decimal Balance => _balance;

    public void Deposit(decimal amount)
    {
        // TODO(you)
        throw new NotImplementedException("Day 1 task — write this yourself.");
    }

    public void Withdraw(decimal amount)
    {
        // TODO(you): roll the window if the day changed, then enforce BOTH invariants.
        throw new NotImplementedException("Day 1 task — write this yourself.");
    }
}

#endregion

#region Test harness

public static class Program
{
    public static void Main()
    {
        Section("STEP 1 — watch the naive versions break");

        var naive = new NaiveBankAccount { Balance = -5_000m };
        Console.WriteLine($"  NaiveBankAccount.Balance = {naive.Balance}   <-- private field, still broken");

        var half = new HalfGuardedBankAccount();
        half.Deposit(-5_000m);
        Console.WriteLine($"  HalfGuarded via Deposit(-5000) = {half.Balance}   <-- back door");

        Section("STEP 2 — the guarded version holds");

        var acc = new BankAccount(1_000m);
        acc.Deposit(500m);
        acc.Withdraw(200m);
        Console.WriteLine($"  Balance after +500 / -200 on 1000 = {acc.Balance}   (expect 1300)");

        ExpectThrow("new BankAccount(-1)",        () => _ = new BankAccount(-1m));
        ExpectThrow("Deposit(-5000)",             () => acc.Deposit(-5_000m));
        ExpectThrow("Withdraw(0)",                () => acc.Withdraw(0m));
        ExpectThrow("Withdraw(999999) overdraft", () => acc.Withdraw(999_999m));

        Section("STEP 3 — Money value object");

        Console.WriteLine($"  Money.Positive(250.5) = {Money.Positive(250.5m)}");
        ExpectThrow("Money.Positive(-1)", () => _ = Money.Positive(-1m));

        Section("STEP 4 — the collection leak");

        var leaky = new LeakyOrder();
        leaky.Lines.Add(new OrderLine("SKU-1", 1));
        leaky.Lines.Clear();
        Console.WriteLine($"  LeakyOrder: caller added then cleared internal state. Count = {leaky.Lines.Count}");

        var order = new Order();
        order.AddLine(new OrderLine("SKU-1", 2));
        Console.WriteLine($"  Order.Lines is IReadOnlyList, count = {order.Lines.Count} — no Add/Clear available to callers");

        Section("STEP 5 — your task");

        try
        {
            var capped = new CappedBankAccount(200_000m, new SystemClock());
            capped.Withdraw(30_000m);
            capped.Withdraw(30_000m);   // should breach the 50k daily cap
            Console.WriteLine("  ❌ daily cap was NOT enforced — invariant #2 is broken");
        }
        catch (NotImplementedException)
        {
            Console.WriteLine("  ⏳ CappedBankAccount not written yet — that's today's homework.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  ✅ daily cap enforced: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Self-check: all fields are private and encapsulation can STILL be broken — how?");
        Console.WriteLine("(unguarded setter · unguarded constructor · leaked mutable reference)");
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
    }

    private static void ExpectThrow(string label, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"  ❌ {label} did NOT throw — invariant is unprotected");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✅ {label} -> {ex.GetType().Name}");
        }
    }
}

#endregion

