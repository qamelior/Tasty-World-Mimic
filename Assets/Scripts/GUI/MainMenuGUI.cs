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
        private Action _onStart;
        public event Action OnStart
        {
            add => _onStart += value;
            remove => _onStart -= value;
        }

        protected override void OnEnable()
        {
            base.OnEnable(); 
            _menuBackground = _root.FindVisualElement<VisualElement>(MenuRootID);
            _menuBackground.SetVisibility(true);
            _onStart += () => gameObject.SetActive(false);
            _root.FindVisualElement<Button>(StartLevelButtonID)?.RegisterCallback<ClickEvent>(evt =>_onStart?.Invoke());
        }
    }
}
