using UnityEngine;
using Zenject;

namespace GUI
{
    public class GUIInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuGUI _mainMenuGUI;
        [SerializeField] private LevelGUI _levelGUI;

        public override void InstallBindings()
        {
            Container.Bind<LevelGUI>().FromInstance(_levelGUI);
            Container.Bind<MainMenuGUI>().FromInstance(_mainMenuGUI);
        }
    }
}
