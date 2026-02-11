public interface ISaveLoadService
{
    string GetSavePath(string profileName);
    void SaveWallet(WalletService wallet, string profileName);
    void LoadWallet(WalletService wallet, string profileName);
}
