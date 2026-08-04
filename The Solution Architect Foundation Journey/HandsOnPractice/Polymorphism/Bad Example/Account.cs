

namespace Polymorphism.Bad_Example;

public class Account
{
    private decimal _balance;
    public decimal Balance => _balance;

    public Account(decimal balence)
    {
        if (balence < 0)
        {
            throw new ArgumentException("Balance cannot be negative");
        }
        _balance = balence;
    }

    public decimal MonthlyInterest()
    {
        return 0;
    }
}
