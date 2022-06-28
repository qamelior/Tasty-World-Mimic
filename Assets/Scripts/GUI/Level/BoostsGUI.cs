using System;
using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace GUI.Level
{
    public class BoostsGUI : GUIWindow
    {
        [SerializeField] private string _useBoostButtonID = "GetFixButton";
        [SerializeField] private string _boostCounterID = "BoostsCounter";
        private Label _boostsNumber;
        private Action _onSpendBoostClick;
        public event Action OnSpendBoostClick { add => _onSpendBoostClick += value; remove => _onSpendBoostClick -= value; }

        [Inject]
        public override void Construct()
        {
            base.Construct();
            _boostsNumber = _root.FindVisualElement<Label>(_boostCounterID);


            _root.RegisterButtonEvent(_useBoostButtonID, _ => _onSpendBoostClick?.Invoke());
        }

        public void UpdateNumberOfBoosts(int value) { _boostsNumber.text = $"{value}"; }
    }
}