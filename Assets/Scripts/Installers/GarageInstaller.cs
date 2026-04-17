using UnityEngine;
using Zenject;

public class GarageInstaller : MonoInstaller
{

    [SerializeField] private CarsDatabase _carDatabase; 
    public override void InstallBindings()
    {
        // not the best solution, should be replaced by dynamic loading
        Container.BindInstance(_carDatabase).WithId("CarDatabase");

        // -- Garage Controller --
        Container.Bind<GarageController>().AsSingle();
        // -- Garage UI -- 
        Container.Bind<GarageUIConroller>().AsSingle();

    }
}