using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MoneyVis : MonoBehaviour
{
    private IWalletService _walletService;
    private ProfileManager _profileManager;
    [SerializeField] private Currency _currency;
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Button _depositMoney;
    [SerializeField] private Button _withdrawMoney;
    

    [Inject]
    public void Construct(ProfileManager profileManager)
    {
        //_walletService = walletService;

        _profileManager = profileManager;

        if (_profileManager.CurrentProfile == null)
        {
            if (_profileManager.Profiles.Count > 0)
            {
                _profileManager.LoadProfile(_profileManager.Profiles[0]);
            }
            else
            {
                _profileManager.CreateProfile("DefaultProfile");
            }
        }

        //_walletService.OnBalanceChanged += OnBalanceChanged;
        _depositMoney?.onClick.AddListener(OnDepositMoneyClicked);
        _withdrawMoney?.onClick.AddListener(OnWithdrawMoneyClicked);
    }

    void Start()
    {
        //var cash = _walletService.GetCurentMoneyByType(_currency);
    
        if (_profileManager == null)
        {
            Debug.LogError("_profileManager is NULL!");
            return;
        }
        
        if (_profileManager.CurrentProfile == null)
        {
            Debug.LogError("CurrentProfile is NULL!");
            return;
        }
        
        if (_profileManager.CurrentProfile.Wallet == null)
        {
            Debug.LogError("Wallet is NULL!");
            return;
        }   

        _profileManager.CurrentProfile.Wallet.TransactionTry(Currency.Cash, 10000L);

        var cash = _profileManager.CurrentProfile.Wallet.GetCurentMoneyByType(_currency);
        

        _label.text = $"{_currency}: {cash}";
        
    }

    void OnDestroy()
    {
        //_walletService.OnBalanceChanged -= OnBalanceChanged;
        _depositMoney?.onClick.RemoveListener(OnDepositMoneyClicked);
        _withdrawMoney?.onClick.RemoveListener(OnWithdrawMoneyClicked);
    }

    private void OnDepositMoneyClicked()
    {
        _walletService.TransactionTry(_currency, 100);
    }

    private void OnWithdrawMoneyClicked()
    {
        if (!_walletService.TransactionTry(_currency, -100))
            Debug.LogWarning($"Not enough {_currency}!");
    }

    private void OnBalanceChanged(Currency currency, long cash)
    {
        _label.text = $"{currency}: {cash}";
    }
}
