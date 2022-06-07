using System;
using System.Collections.Generic;

namespace Restaurants.Customers;

[Serializable]
public class CustomerOrder
{
    private List<string> _mealsToDeliver;
    private Action<string> _onMealDelivered;
    private Action _onOrderFulfilled;

    public CustomerOrder(CustomerOrderPreset p)
    {
        Preset = p;
        InitFulfillment();

        void InitFulfillment()
        {
            _mealsToDeliver = new List<string>();
            foreach (var meal in Preset.Meals)
                _mealsToDeliver.Add(meal.UID);
        }
    }

    public CustomerOrderPreset Preset { private set; get; }

    public event Action<string> OnMealDelivered { add => _onMealDelivered += value; remove => _onMealDelivered -= value; }

    public event Action OnOrderFulfilled { add => _onOrderFulfilled += value; remove => _onOrderFulfilled -= value; }

    public bool TryDeliverMeal(string mealID)
    {
        bool hasDelivered = _mealsToDeliver.Remove(mealID);
        if (!hasDelivered) return false;
        _onMealDelivered?.Invoke(mealID);
        if (_mealsToDeliver.Count == 0)
            _onOrderFulfilled?.Invoke();
        return true;
    }

    public void ForceComplete()
    {
        _mealsToDeliver.Clear();
        _onOrderFulfilled?.Invoke();
    }
}