using TMPro;
using UnityEngine;
using Zenject;

public class MoneyVis : MonoBehaviour
{

    [Inject] private WalletService _walletService;
    [SerializeField] private TextMeshProUGUI _text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _walletService.OnBalanceChanged += OnBalanceChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        _walletService.OnBalanceChanged -= OnBalanceChanged;
    }

    private void OnBalanceChanged(Currency currency, long cash)
    {
        _text.text = $"{currency}: {cash}";
    }
}
