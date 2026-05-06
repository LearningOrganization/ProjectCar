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

        _garageController.OnCarChanged += OnChangeCar;
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    private void OnChangeCar(int carId, string carPrefabName)
    {
        CarIdText.text = carId.ToString();
        CarPrefabName.text = carPrefabName;
    }

    private void OnDestroy()
    {
        _garageController.OnCarChanged -= OnChangeCar;
    }
}
