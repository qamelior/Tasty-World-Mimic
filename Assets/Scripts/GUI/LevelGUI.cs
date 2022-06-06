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

        private Label _levelTimer;
        private Label _customersNumber;
        private VisualElement _menu;
        private Label _menuHeader;

        private Action<MenuMode> _onMenuToggle;
        private Action _onGetBoost;
        private Action _onRestart;
        private Action _onQuit;
        public event  Action<MenuMode> OnMenuToggle
        {
            add => _onMenuToggle += value;
            remove => _onMenuToggle -= value;
        }
        public event Action OnGetBoost
        {
            add => _onGetBoost += value;
            remove => _onGetBoost -= value;
        }
        public event Action OnRestart
        {
            add => _onRestart += value;
            remove => _onRestart -= value;
        }
        public event Action OnQuit
        {
            add => _onQuit += value;
            remove => _onQuit -= value;
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
            _onRestart += () => ToggleMenu(MenuMode.Closed);
            _root.FindVisualElement<Button>(_openMenuButtonID)?.RegisterCallback<ClickEvent>(evt => ToggleMenu(MenuMode.Paused));
            _root.FindVisualElement<Button>(_continueButtonID)?.RegisterCallback<ClickEvent>(evt => ToggleMenu(MenuMode.Closed));
            _root.FindVisualElement<Button>(_getBoostButtonID)?.RegisterCallback<ClickEvent>(evt => _onGetBoost?.Invoke());
            _root.FindVisualElement<Button>(_restartButtonID)?.RegisterCallback<ClickEvent>(evt => _onRestart?.Invoke());
            _root.FindVisualElement<Button>(_quitButtonID)?.RegisterCallback<ClickEvent>(evt => _onQuit?.Invoke());
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
            _onMenuToggle?.Invoke(mode);
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
