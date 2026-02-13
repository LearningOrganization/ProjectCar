using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MoneyVis : MonoBehaviour
{
    private IWalletService _walletService;
    [SerializeField] private Currency _currency;
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Button _depositMoney;
    [SerializeField] private Button _withdrawMoney;

    [Inject]
    public void Construct(IWalletService walletService)
    {
        _walletService = walletService;
        _walletService.OnBalanceChanged += OnBalanceChanged;
        _depositMoney?.onClick.AddListener(OnDepositMoneyClicked);
        _withdrawMoney?.onClick.AddListener(OnWithdrawMoneyClicked);
    }

    void Start()
    {
        var cash = _walletService.GetCurentMoneyByType(_currency);
        _label.text = $"{_currency}: {cash}";
    }

    void OnDestroy()
    {
        _walletService.OnBalanceChanged -= OnBalanceChanged;
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
