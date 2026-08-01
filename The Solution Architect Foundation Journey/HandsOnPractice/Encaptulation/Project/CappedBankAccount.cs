// YOUR TASK — daily withdrawal cap (an invariant spanning multiple doors)

// Rule: total withdrawals in a single day must not exceed 50,000.
//
// Questions to answer with your design (this is the real lesson):
//   - Where does "today's withdrawn total" live?  (Hint: inside, always inside.)
//   - Who resets it? If the caller can reset it, the caller can bypass the rule.
//   - How does the object know what "today" is without you passing it in every call
//     (which the caller could lie about)?  -> inject a clock abstraction. That's DIP,
//     Day 17. Notice how the principles start pulling on each other.

namespace Encaptulation.Project;


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
        ArgumentOutOfRangeException.ThrowIfNegative(openingBalance);

        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _balance = openingBalance;
        _windowDate = _clock.Today;
        _withdrawnToday = 0;
    }

    public decimal Balance => _balance;

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be greater than 0");
        }
        _balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        if (amount > _balance)
        {
            throw new InvalidOperationException("Insufficient balance.");
        }

        ResetDailyWindowIfNeeded(_clock.Today);

        if (_withdrawnToday + amount > DailyWithdrawalLimit)
        {
            throw new InvalidOperationException($"Daily withdrawal limit of {DailyWithdrawalLimit:N2} exceeded.");
        }
        _withdrawnToday += amount;
        _balance -= amount;
    }

    private void ResetDailyWindowIfNeeded(DateOnly today)
    {
        if (today == _windowDate)
            return;

        _windowDate = today;
        _withdrawnToday = 0m;
    }
}


