using System;
using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUI
{
    public class MainMenuGUI : GUIWindow
    {
        [Header("Element IDs")]
        [SerializeField] private string MenuRootID = "MainMenu";
        [SerializeField] private string StartLevelButtonID = "StartLevelButton";
    
        private VisualElement _menuBackground;
        private Action _onStartButtonClicked;
        public event Action OnStartButtonClicked
        {
            add => _onStartButtonClicked += value;
            remove => _onStartButtonClicked -= value;
        }

        protected override void OnEnable()
        {
            base.OnEnable(); 
            _menuBackground = _root.FindVisualElement<VisualElement>(MenuRootID);
            _menuBackground.SetVisibility(true);
            _onStartButtonClicked += () => _menuBackground.SetVisibility(false);
            _root.FindVisualElement<Button>(StartLevelButtonID)?.RegisterCallback<ClickEvent>(evt =>_onStartButtonClicked?.Invoke());
        }
    }
}
