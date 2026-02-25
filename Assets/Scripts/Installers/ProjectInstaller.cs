using Zenject;

namespace PawnShop.Installers
{
    public class ProjectInstaller : MonoInstaller
    {

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void InstallBindings()
        {
            // --- Core Game ---
            Container.Bind<GameBootstrapper>().FromComponentInHierarchy().AsSingle();
            Container.Bind<StateMachine<IGameState>>().AsSingle();
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.Bind<ProfileManager>().AsSingle();

            // --- Game States ---
            Container.Bind<GamePlayState>().AsSingle();
            Container.Bind<LoadingState>().AsSingle();
            Container.Bind<MainMenuState>().AsSingle();
            Container.Bind<SettingsState>().AsSingle();

        }
    }
}