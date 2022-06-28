using System;
using _Extensions;
using Game.Data.Levels;
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
        private Action _onStartClick;
        public event Action OnStartClick { add => _onStartClick += value; remove => _onStartClick -= value; }
        [Inject]
        public override void Construct()
        {
            base.Construct();
            _menuBackground = _root.FindVisualElement<VisualElement>(MenuRootID);
            _menuBackground.Show();
            _root.RegisterButtonEvent(StartLevelButtonID, evt => _onStartClick?.Invoke());
            _onStartClick += () => gameObject.SetActive(false);
        }
    }
}