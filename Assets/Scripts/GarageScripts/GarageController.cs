using System;
using UnityEngine;
using Zenject;

public class GarageController : MonoBehaviour
{

    [HideInInspector] public event Action<int> OnCarChanged;

    private CarContainer _carContainer;
    private CarsDatabase _carDatabase;

    [Inject]
    public void Construct([Inject(Id = "CarContainer")]CarContainer carContainer, [Inject(Id = "CarDatabase")]CarsDatabase carsDatabase)
    {
        _carContainer = carContainer;
        _carDatabase = carsDatabase;
    }

    private void Awake()
    {
        _carContainer.Car = _carDatabase.Cars[0];
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
