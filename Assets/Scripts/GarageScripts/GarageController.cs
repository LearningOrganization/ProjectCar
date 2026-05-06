using System;
using UnityEngine;
using Zenject;

public class GarageController : MonoBehaviour
{
    [HideInInspector] public event Action<int, string> OnCarChanged;
    [HideInInspector] public event Action<int> ChangeCarPlatform;
    private CarContainer _carContainer;
    private CarsDatabase _carDatabase;
    private int _currCarID = 0;
    private float _timer;
    
    private Vector3 _targetPos;
    private bool _isMoving;

    [Inject]
    public void Construct(
        [Inject(Id = "CarContainer")]CarContainer carContainer,
        [Inject(Id = "CarDatabase")]CarsDatabase carsDatabase)
    {
        _carContainer = carContainer;
        _carDatabase = carsDatabase;
    }

    public void ChangeCar(bool toNext)
    {
        if(toNext)
        {
            if(_currCarID < _carDatabase.Cars.Length - 1)
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
        ChangeCarPlatform?.Invoke(_currCarID);

    }

    private void GarageInitCall()
    {
        _currCarID = 0;
        _carContainer.Car = _carDatabase.Cars[_currCarID];

        OnCarChanged?.Invoke(_currCarID, _carContainer.Car.name);
        ChangeCarPlatform?.Invoke(_currCarID);
    }

    private void Start()
    {
        GarageInitCall();
    }

    private void Update()
    {
        
    }
}
