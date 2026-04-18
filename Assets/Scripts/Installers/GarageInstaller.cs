using UnityEngine;
using Zenject;

public class GarageInstaller : MonoInstaller
{

    [SerializeField] private CarsDatabase _carDatabase; 
    public override void InstallBindings()
    {
        // not the best solution, should be replaced by dynamic loading
        Container.BindInstance(_carDatabase).WithId("CarDatabase").AsSingle();

        // -- Garage Controller --
        Container.Bind<GarageController>().FromComponentInHierarchy().AsSingle();
        // -- Garage UI -- 
        Container.Bind<GarageUIConroller>().FromComponentInHierarchy().AsSingle();

    }
}