using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    public void SaveWallet(string path)
    {
        WalletData data = new WalletData();
        foreach (var kvp in _currencies)
        {
            data.entries.Add(new WalletEntry(kvp.Key.ToString(), kvp.Value));
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Wallet saved to: " + path);
    }
    public void LoadWallet(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("Wallet file not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        WalletData data = JsonUtility.FromJson<WalletData>(json);

        _currencies.Clear();
        foreach (var entry in data.entries)
        {
            if (Enum.TryParse<Currency>(entry.currencyName, out Currency currency))
            {
                _currencies[currency] = entry.amount;
                OnBalanceChanged?.Invoke(currency, entry.amount);
            }
            else
            {
                Debug.LogWarning("Unknown currency in save: " + entry.currencyName);
            }
        }
    }

}

public enum Currency
{
    Cash,
    RepairToken,
    TotaledCarRepairingToken,
    FreeCarToken
}
