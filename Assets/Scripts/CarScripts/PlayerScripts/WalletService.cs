using System;
using System.Collections.Generic;
using UnityEngine;

public class WalletService
{
    public event Action<Currency, long> OnBalanceChanged; 
    private Dictionary<Currency, long> _currencies = new();

    public WalletService(Currency currency, long amount)
    {
        _currencies[currency] = amount;
    }

    public bool TransactionTry(Currency currency, long count)
    {
            var currentMoneyCopy = GetCurentMoneyByType(currency);

            var result = currentMoneyCopy + count;

            if(result < 0)
            {
                return false;
            }

            _currencies[currency] = result;

            OnBalanceChanged?.Invoke(currency, result);

            return true;     
            
    }

    public long GetCurentMoneyByType(Currency currency)
    {
        if(_currencies.TryGetValue(currency, out long value))
            {return value;}
        
        return 0;
    }
    
}

public enum Currency
{
    Cash, 
    Tokens
}
