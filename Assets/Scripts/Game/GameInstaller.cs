using Game.Data.Levels;
using Zenject;

namespace Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle();
        }
    }
}