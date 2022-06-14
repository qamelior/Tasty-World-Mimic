using System;
using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace GUI
{
    public class MainMenuGUI : GUIWindow
    {
        [SerializeField] private string MenuRootID = "MainMenu";
        [SerializeField] private string StartLevelButtonID = "StartLevelButton";
        private VisualElement _menuBackground;
        private Action _onStart;
        public event Action OnStart { add => _onStart += value; remove => _onStart -= value; }

        [Inject]
        public override void Construct()
        {
            base.Construct();
            _menuBackground = _root.FindVisualElement<VisualElement>(MenuRootID);
            _menuBackground.Show();
            _onStart += () => gameObject.SetActive(false);
            _root.RegisterButtonEvent(StartLevelButtonID, evt => _onStart?.Invoke());
        }
    }
}