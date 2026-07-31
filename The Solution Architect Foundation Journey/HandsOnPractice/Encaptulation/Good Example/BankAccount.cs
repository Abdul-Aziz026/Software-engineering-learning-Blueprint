
namespace Encaptulation;

// The invariant-protected version

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
