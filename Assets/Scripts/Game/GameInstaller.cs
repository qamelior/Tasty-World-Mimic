using Game.Data.Levels;
using Zenject;

namespace Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            InstallLevelTools();
        }

        private void InstallLevelTools()
        {
            Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<Timer>().AsSingle();
            Container.BindInterfacesAndSelfTo<CustomerCounter>().AsSingle();
            Container.BindInterfacesAndSelfTo<Booster>().AsSingle();
        }
    }
}