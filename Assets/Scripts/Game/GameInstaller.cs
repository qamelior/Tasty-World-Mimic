using Game.Data.Levels;
using Zenject;

namespace Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.Bind<LevelManager>().AsSingle();
            // ExecutionOrder();
            // void ExecutionOrder()
            // {
            //     Container.BindExecutionOrder<GameController>(-10);
            //     //Container.BindExecutionOrder<LevelManager>(-10);
            // }
        }
    }
}
