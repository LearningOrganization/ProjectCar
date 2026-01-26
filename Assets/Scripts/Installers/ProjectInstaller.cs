using Zenject;

namespace PawnShop.Installers
{
    public class ProjectInstaller : MonoInstaller
    {

        private void Awake()
        {
            // Make this installer persist between scenes
            DontDestroyOnLoad(gameObject);
        }

        public override void InstallBindings()
        {
            // --- Core Game ---
            Container.Bind<GameBootstrapper>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.Bind<WalletService>().AsSingle().WithArguments(Currency.Cash, 10000);

            // --- Game States ---
            Container.Bind<GamePlayState>().AsSingle();
            Container.Bind<LoadingState>().AsSingle();
            Container.Bind<MainMenuState>().AsSingle();
            Container.Bind<SettingsState>().AsSingle();
        }
    }
}