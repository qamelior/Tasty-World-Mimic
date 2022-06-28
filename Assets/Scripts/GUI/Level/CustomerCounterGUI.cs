using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace GUI.Level
{
    public class CustomerCounterGUI : GUIWindow
    {
        [SerializeField] private string _customersNumberID = "CustomerCount";
        private Label _customersNumber;

        [Inject]
        public override void Construct()
        {
            base.Construct();
            _customersNumber = _root.FindVisualElement<Label>(_customersNumberID);
        }

        public void UpdateCustomersNumber(Vector2Int customersNumber)
        {
            _customersNumber.text = $"{customersNumber.x}/{customersNumber.y}";
        }
    }
}