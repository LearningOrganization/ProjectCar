public class Profile
{
    public string ProfileName;
    public WalletService Wallet;
    
    // for future
    // public Cars Cars;
    // public CarUpdates Updates;

    public Profile(string name)
    {
        ProfileName = name;
        Wallet = new WalletService();
    }
}