using System;
using UnityEngine;
using Zenject;

public class GarageController : MonoBehaviour
{

    [HideInInspector] public event Action<int, string> OnCarChanged;

    private CarContainer _carContainer;
    private CarsDatabase _carDatabase;
    private int _currCarID;

    [Inject]
    public void Construct(
        [Inject(Id = "CarContainer")]CarContainer carContainer,
        [Inject(Id = "CarDatabase")]CarsDatabase carsDatabase)
    {
        _carContainer = carContainer;
        _carDatabase = carsDatabase;
        _currCarID = 0;
    }

    private void Awake()
    {
        //_carContainer.Car = _carDatabase.Cars[1];
    }

    public void ChangeCar(bool toNext)
    {

        if(toNext)
        {
            if(_currCarID < _carDatabase.Cars.Length)
            {
                _currCarID ++;
            }
        }
        else
        {
            if(_currCarID > 0)
            {
                _currCarID --;
            }
        }
        _carContainer.Car = _carDatabase.Cars[_currCarID];
        OnCarChanged?.Invoke(_currCarID, _carContainer.Car.name);

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
