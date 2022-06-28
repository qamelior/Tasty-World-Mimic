using GUI.Level;
using UnityEngine;
using Zenject;

namespace GUI
{
    public class GUIInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuGUI _mainMenuGUI;
        [SerializeField] private BoostsGUI _boostsGUI;
        [SerializeField] private CustomerCounterGUI _customerCounterGUI;
        [SerializeField] private MenuGUI _menuGUI;
        [SerializeField] private TimerGUI _timerGUI;

        public override void InstallBindings()
        {
            Container.Bind<MainMenuGUI>().FromInstance(_mainMenuGUI);
            Container.Bind<BoostsGUI>().FromInstance(_boostsGUI);
            Container.Bind<CustomerCounterGUI>().FromInstance(_customerCounterGUI);
            Container.Bind<MenuGUI>().FromInstance(_menuGUI);
            Container.Bind<TimerGUI>().FromInstance(_timerGUI);
        }
    }
}
