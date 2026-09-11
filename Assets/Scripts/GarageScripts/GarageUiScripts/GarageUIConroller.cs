using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GarageUIConroller : MonoBehaviour
{
    [SerializeField] private Button RightButton;
    [SerializeField] private Button LeftButton;
    [SerializeField] private TMP_Text CarIdText;
    [SerializeField] private TMP_Text CarPrefabName;

    [SerializeField] private TMP_Text CarCountLabel;

    [SerializeField] private Toggle IsBatchProcessing;
    [SerializeField] private Slider CarsCountSlider;

    private GarageController _garageController;

    [Inject]
    public void Construct(GarageController garageController)
    {
        _garageController = garageController;
    }

    void Awake()
    {
        RightButton.onClick.AddListener(() => _garageController.ChangeCar(true));
        LeftButton.onClick.AddListener(() => _garageController.ChangeCar(false));

        IsBatchProcessing.onValueChanged.AddListener(isOn => _garageController.ChangeProcessingType(isOn));
        CarsCountSlider.onValueChanged.AddListener(value => _garageController.ChangeCarAmount(((int)value)));

        
        _garageController.OnChangeCarAmount += OnChangeCarAmount;
        _garageController.OnCarChanged += OnChangeCar;
    }

    void Start()
    {
        // not the best solution, but anyway
        _garageController.ChangeProcessingType(IsBatchProcessing.isOn);
        _garageController.ChangeCarAmount((int)CarsCountSlider.value);
    }
    void Update()
    {
        
    }

    private void OnChangeCar(int carId, string carPrefabName)
    {
        CarIdText.text = carId.ToString();
        CarPrefabName.text = carPrefabName;
    }

    private void OnChangeCarAmount(int carCount)
    {
        CarCountLabel.text = carCount.ToString();
    }

    private void OnDestroy()
    {
        _garageController.OnChangeCarAmount -= OnChangeCarAmount;
        _garageController.OnCarChanged -= OnChangeCar;
    }
}
