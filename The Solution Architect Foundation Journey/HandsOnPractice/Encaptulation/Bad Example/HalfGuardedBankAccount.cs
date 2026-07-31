
namespace Encaptulation;

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