// Day 01 — Encapsulation as invariant protection
// Journey: The Solution Architect Foundation Journey (Day 1 of 90)
using Encaptulation;
using System;
using System.Collections.Generic;

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

        var naive = new NoGuardBankAccount { Balance = -5_000m };
        Console.WriteLine($"  NoGuardBankAccount.Balance = {naive.Balance}   <-- private field, still broken");

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

