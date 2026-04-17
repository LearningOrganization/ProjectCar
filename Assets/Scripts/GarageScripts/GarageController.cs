using System;
using UnityEngine;
using Zenject;

public class GarageController : MonoBehaviour
{

    [HideInInspector] public event Action OnCarChanged;

    private CarContainer _carContainer;
    private CarsDatabase _carDatabase;

    [Inject]
    public void Construct(CarContainer carContainer, CarsDatabase carsDatabase)
    {
        _carContainer = carContainer;
        _carDatabase = carsDatabase;
    }

    private void Awake()
    {
        _carContainer.Car = _carDatabase.Cars[1];
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
