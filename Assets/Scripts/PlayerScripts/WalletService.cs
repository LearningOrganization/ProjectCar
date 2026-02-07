using System;
using System.Collections.Generic;

public interface IWalletService
{
    event Action<Currency, long> OnBalanceChanged;
    bool TransactionTry(Currency currency, long count);
    long GetCurentMoneyByType(Currency currency);
}

public class WalletService : IWalletService
{
    public event Action<Currency, long> OnBalanceChanged;
    private Dictionary<Currency, long> _currencies = new();

    public bool TransactionTry(Currency currency, long count)
    {
        var currentMoneyCopy = GetCurentMoneyByType(currency);
        var result = currentMoneyCopy + count;
        if (result < 0)
        {
            return false;
        }
        _currencies[currency] = result;
        OnBalanceChanged?.Invoke(currency, result);
        return true;
    }

    public long GetCurentMoneyByType(Currency currency)
    {
        if (_currencies.TryGetValue(currency, out long value))
        {
            return value;
        }
        return 0;
    }
}

public enum Currency
{
    Cash,
    RepairToken,
    TotaledCarRepairingToken,
    FreeCarToken
}
