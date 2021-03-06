using _Extensions;
using Restaurants.Customers.Orders;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUI
{
    public class CustomerOrderGUI : MonoBehaviour
    {
        private OrderMealUI[] _mealUI;
        private VisualElement _root;
        private UIDocument _ui;

        public void Init()
        {
            _ui = GetComponent<UIDocument>();
            _root = _ui.rootVisualElement;
            _mealUI = new[]
            {
                new OrderMealUI(_root, "First", "FirstLabel"),
                new OrderMealUI(_root, "Second", "SecondLabel"),
                new OrderMealUI(_root, "Third", "ThirdLabel")
            };
            Hide();
        }

        public void ShowOrder(Order order)
        {
            _root.Show();
            var orderedMeals = order.PresetSO.Meals;
            for (var i = 0; i < _mealUI.Length; i++)
                if (i < orderedMeals.Length)
                    _mealUI[i].Show(orderedMeals[i].DisplayName, orderedMeals[i].UID);
                else
                    _mealUI[i].Hide();
        }

        public void Hide() { _root.Hide(); }

        public void OnMealDelivered(string mealID)
        {
            foreach (var elem in _mealUI)
                if (elem.IsVisible && elem.MealID == mealID)
                {
                    elem.Hide();
                    return;
                }
        }

        private class OrderMealUI
        {
            private readonly VisualElement _container;
            private readonly Label _label;

            public OrderMealUI(VisualElement root, string containerID, string labelID)
            {
                _container = root.Q<VisualElement>(containerID);
                _label = root.Q<Label>(labelID);
            }

            public string MealID { private set; get; }
            public bool IsVisible => _container.IsVisible();

            public void Show(string mealName, string id)
            {
                _container.Show();
                _label.text = mealName;
                MealID = id;
            }

            public void Hide() { _container.Hide(); }
        }
    }
}