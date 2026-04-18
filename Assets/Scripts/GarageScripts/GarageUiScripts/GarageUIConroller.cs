using System;
using UnityEngine;
using Zenject;

public class GarageUIConroller : MonoBehaviour
{
    private GarageController _garageController;

    [Inject]
    public void Construct(GarageController garageController)
    {
        _garageController = garageController;
    }

    void Awake()
    {
        _garageController.OnCarChanged += ChangeCar;
    }

    private void ChangeCar(int carID)
    {
        
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    void OnDestroy()
    {
        _garageController.OnCarChanged -= ChangeCar;
    }
}
