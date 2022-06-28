using System.Collections.Generic;
using Restaurants.Customers;
using Restaurants.Customers.Orders;
using UnityEngine;

// ReSharper disable LoopCanBeConvertedToQuery
//LINQ is expensive

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Food/Collection")]
    public class FoodCollection : ScriptableObject
    {
        public List<MealPreset> Meals;

        [SerializeField] private OrderPresetSO[] _orderPresets;
        public OrderPresetSO[] OrderPresets => _orderPresets;

        //TODO call this func
        public void ValidateCollection()
        {
            for (var i = 0; i < Meals.Count; i++)
            for (int j = i + 1; j < Meals.Count; j++)
                if (Meals[i].UID == Meals[j].UID)
                    Debug.LogError($"[FoodCollection] Same UID ({Meals[i].UID}) used for different meals");
        }

        public bool IsValidID(string id)
        {
            var preset = GetPreset(id);
            return preset != null;
        }

        public OrderPresetSO GetPreset(string id)
        {
            foreach (var preset in _orderPresets)
                if (preset.UID == id)
                    return preset;
            return null;
        }

        public OrderPresetSO SelectRandomPreset(int mealsNumber)
        {
            var selection = new List<OrderPresetSO>();
            foreach (var preset in _orderPresets)
                if (preset.Meals.Length == mealsNumber)
                    selection.Add(preset);
            return selection[Random.Range(0, selection.Count - 1)];
        }
    }
}