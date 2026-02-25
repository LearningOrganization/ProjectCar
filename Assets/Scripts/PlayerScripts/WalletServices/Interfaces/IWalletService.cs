using System;
public interface IWalletService
{
    event Action<Currency, long> OnBalanceChanged;
    bool TransactionTry(Currency currency, long count);
    long GetCurentMoneyByType(Currency currency);
}