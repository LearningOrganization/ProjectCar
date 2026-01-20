

using Zenject;

public class GameStatesInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // states and fsm
        Container.Bind<GameStateMachine>().AsSingle();
        Container.Bind<GamePlayState>().AsSingle();
        Container.Bind<LoadingState>().AsSingle();
        Container.Bind<MainMenuState>().AsSingle();
        Container.Bind<SettingsState>().AsSingle();

        //scene loader interface 
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
        //game bootstrapper
        Container.Bind<GameBootstrapper>().FromComponentInHierarchy().ToString();

    }
}