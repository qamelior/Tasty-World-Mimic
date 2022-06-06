using System;
using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUI
{
    public class LevelGUI : GUIWindow
    {
        [Header("Element IDs")]
        [SerializeField] private string _levelTimerID = "LevelTimer";
        [SerializeField] private string _customersNumberID = "CustomerCount";
        
        [SerializeField] private string _openMenuButtonID = "PauseButton";
        [SerializeField] private string _menuID = "Menu";
        [SerializeField] private string _menuHeaderID = "MenuHeader";
        
        [SerializeField] private string _continueButtonID = "Continue";
        [SerializeField] private string _getBoostButtonID = "GetBoost";
        [SerializeField] private string _restartButtonID = "RestartButton";
        [SerializeField] private string _quitButtonID = "QuitButton";
        
        [SerializeField] private string _useBoostButtonID = "GetFixButton";
        [SerializeField] private string _boostCounterID = "BoostsCounter";

        private Label _levelTimer;
        private Label _customersNumber;
        private VisualElement _menu;
        private Label _menuHeader;
        private Label _boostsNumber;

        private Action<MenuMode> _onMenuModeSwitch;
        private Action _onGetBoostClick;
        private Action _onSpendBoostClick;
        private Action _onRestartClick;
        private Action _onQuitClick;
        public event  Action<MenuMode> OnMenuModeSwitch
        {
            add => _onMenuModeSwitch += value;
            remove => _onMenuModeSwitch -= value;
        }
        public event Action OnGetBoostClick
        {
            add => _onGetBoostClick += value;
            remove => _onGetBoostClick -= value;
        }
        public event Action OnSpendBoostClick
        {
            add => _onSpendBoostClick += value;
            remove => _onSpendBoostClick -= value;
        }
        public event Action OnRestartClick
        {
            add => _onRestartClick += value;
            remove => _onRestartClick -= value;
        }
        public event Action OnQuitClick
        {
            add => _onQuitClick += value;
            remove => _onQuitClick -= value;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _levelTimer = _root.FindVisualElement<Label>(_levelTimerID);
            _customersNumber = _root.FindVisualElement<Label>(_customersNumberID);
            //level finished window
            _menu = _root.FindVisualElement<VisualElement>(_menuID);
            _menu.SetVisibility(false);
            _menuHeader = _root.FindVisualElement<Label>(_menuHeaderID);
            _onRestartClick += () => ToggleMenu(MenuMode.Closed);
            _root.FindVisualElement<Button>(_openMenuButtonID)?.RegisterCallback<ClickEvent>(evt => ToggleMenu(MenuMode.Paused));
            _root.FindVisualElement<Button>(_continueButtonID)?.RegisterCallback<ClickEvent>(evt => ToggleMenu(MenuMode.Closed));
            _root.FindVisualElement<Button>(_getBoostButtonID)?.RegisterCallback<ClickEvent>(evt => _onGetBoostClick?.Invoke());
            _root.FindVisualElement<Button>(_restartButtonID)?.RegisterCallback<ClickEvent>(evt => _onRestartClick?.Invoke());
            _root.FindVisualElement<Button>(_quitButtonID)?.RegisterCallback<ClickEvent>(evt => _onQuitClick?.Invoke());
            _root.FindVisualElement<Button>(_useBoostButtonID)?.RegisterCallback<ClickEvent>(evt => _onSpendBoostClick?.Invoke());
            _boostsNumber = _root.FindVisualElement<Label>(_boostCounterID);
        }

        public void UpdateLevelTimer(int timeLeftSeconds)
        {
            int minutes = timeLeftSeconds / 60;
            int seconds = timeLeftSeconds % 60;
            _levelTimer.text = $"{minutes:00}:{seconds:00}";
        }
        public void UpdateCustomersNumber(Vector2Int customersNumber)
        {
            _customersNumber.text = $"{customersNumber.x}/{customersNumber.y}";
        }

        public void UpdateNumberOfBoosts(int value)
        {
            _boostsNumber.text = $"{value}";
        }
        
        public void ToggleMenu(MenuMode mode)
        {
            switch (mode)
            {
                case MenuMode.Paused:
                    _menuHeader.text = "Paused"; 
                    break;
                case MenuMode.LevelCompleted:
                    _menuHeader.text = "Level completed!";
                    break;
                case MenuMode.LevelFailed:
                    _menuHeader.text = "Level failed!";
                    break;
            }
            _onMenuModeSwitch?.Invoke(mode);
            _menu.SetVisibility(mode != MenuMode.Closed);
        }


        public enum MenuMode
        {
            Closed,
            Paused,
            LevelCompleted,
            LevelFailed,
        }
    }
}
