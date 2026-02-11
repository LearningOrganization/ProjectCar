[System.Serializable]
public class WalletEntry
{
    public string currencyName;
    public long amount;

    public WalletEntry(string currencyName, long amount)
    {
        this.currencyName = currencyName;
        this.amount = amount;
    }
}