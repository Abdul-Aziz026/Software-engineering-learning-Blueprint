
namespace Encaptulation;

// ❌ private field + public setter. Looks like OOP. Encapsulation is zero.

public class NoGuardBankAccount
{
    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        set => _balance = value;   // no gate. anyone can write anything.
    }
}
