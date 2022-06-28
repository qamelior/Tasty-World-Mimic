using System;
using System.Collections.Generic;
using GUI;

namespace Restaurants.Customers.Orders
{
    public class Order
    {
        private List<string> _mealsToDeliver;
        private Action<string> _onMealDelivered;
        private Action _onOrderFulfilled;
        private CustomerOrderGUI _orderGUI;

        public Order(OrderPresetSO p)
        {
            PresetSO = p;
            InitFulfillment();

            void InitFulfillment()
            {
                _mealsToDeliver = new List<string>();
                foreach (var meal in PresetSO.Meals)
                    _mealsToDeliver.Add(meal.UID);
            }
        }

        public OrderPresetSO PresetSO { private set; get; }

        public void Init(CustomerOrderGUI orderGUI)
        {
            _orderGUI = orderGUI;
            _onMealDelivered += _orderGUI.OnMealDelivered;
        }

        public void Activate() { _orderGUI.ShowOrder(this); }

        public event Action OnOrderFulfilled { add => _onOrderFulfilled += value; remove => _onOrderFulfilled -= value; }

        public bool TryDeliverMeal(string mealID)
        {
            bool hasDelivered = _mealsToDeliver.Remove(mealID);
            if (!hasDelivered) return false;
            _onMealDelivered?.Invoke(mealID);
            if (_mealsToDeliver.Count == 0)
                OnComplete();
            return true;
        }

        private void OnComplete()
        {
            _orderGUI.Hide();
            _onMealDelivered -= _orderGUI.OnMealDelivered;
            _onOrderFulfilled?.Invoke();
            _onOrderFulfilled = null;
        }

        public void ForceComplete()
        {
            _mealsToDeliver.Clear();
            OnComplete();
        }
    }
}