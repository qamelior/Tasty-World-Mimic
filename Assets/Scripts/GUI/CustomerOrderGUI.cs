using _Extensions;
using Restaurants.Customers;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUI
{
    public class CustomerOrderGUI : GUIWindow
    {
        private OrderMealUI[] _mealUI;

        protected override void OnEnable()
        {
            base.OnEnable();

            _mealUI = new[]
            {
                new OrderMealUI(_root, "First", "FirstLabel"),
                new OrderMealUI(_root, "Second", "SecondLabel"),
                new OrderMealUI(_root, "Third", "ThirdLabel"),
            };
        }

        public void ShowAll(CustomerOrder order)
        {
            _root.SetVisibility(true);
            var orderedMeals = order.Preset.Meals;
            for (var i = 0; i < _mealUI.Length; i++)
            {
                if (i < orderedMeals.Length)
                    _mealUI[i].Show(orderedMeals[i].DisplayName, orderedMeals[i].UID);
                else
                    _mealUI[i].Hide();
            }
        }

        public void HideAll() { _root.SetVisibility(false); }

        public void HideMeal(string mealID)
        {
            foreach (var elem in _mealUI)
            {
                if (elem.IsVisible && elem.MealID == mealID)
                {
                    elem.Hide();
                    return;
                }
            }
        }

        private class OrderMealUI
        {
            private readonly VisualElement _container;
            private readonly Label _label;
            public string MealID { private set; get; }
            public bool IsVisible => _container.IsVisible();

            public OrderMealUI(VisualElement root, string containerID, string labelID)
            {
                _container = root.Q<VisualElement>(containerID);
                _label = root.Q<Label>(labelID);
            }

            public void Show(string mealName, string id)
            {
                _container.SetVisibility(true);
                _label.text = mealName;
                MealID = id;
            }

            public void Hide() { _container.SetVisibility(false); }
        }
    }
}
