﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Restaurants.Customers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Data.Levels
{
    [Serializable]
    public class LevelData
    {
        public string DisplayName;
        public LevelType Type;
        public int NumberOfBoosts;
        public int TimeInSeconds;
        [SerializeField] private List<CustomerOrderPreset> _orders;
        public List<string> OrderStrings;
        public int TotalMealsNumber;
        public int CustomersCount;
        public int MaxMealsInOneOrder;
        
        public LevelData(string name)
        {
            DisplayName = name;
        }


        public void Validate()
        {
            TimeInSeconds = Mathf.Max(1, TimeInSeconds);
            switch (Type)
            {
                case LevelType.Fixed:
                    TotalMealsNumber = 0;
                    foreach (var order in _orders)
                        TotalMealsNumber += order.Meals.Length;
                    CustomersCount = _orders.Count;
                    MaxMealsInOneOrder = 0;
                    break;
                
                case LevelType.Random:
                    _orders.Clear();
                    CustomersCount = Mathf.Max(1, CustomersCount);
                    TotalMealsNumber = Mathf.Max(1, TotalMealsNumber);
                    MaxMealsInOneOrder = Mathf.Max(1, MaxMealsInOneOrder);
                    if (CustomersCount > TotalMealsNumber)
                    {
                        Debug.LogError($"Number of customers ({CustomersCount}) is higher than number of dishes ({TotalMealsNumber})");
                        CustomersCount = TotalMealsNumber;
                    }

                    if (MaxMealsInOneOrder * CustomersCount < TotalMealsNumber)
                    {
                        Debug.LogError($"Number of dishes ({TotalMealsNumber}) is less than (number of customers) x (max dishes in one order).)");
                        TotalMealsNumber = MaxMealsInOneOrder * CustomersCount;
                    }
                    break;
            }
        }

        public void SerializeMeals(FoodCollection foodCollection)
        {
            if (Type != LevelType.Fixed)
                return;
            
            OrderStrings = new List<string>();
            for (int i = _orders.Count - 1; i >= 0; i--)
            {
                var order = _orders[i];
                if (order != null && foodCollection.IsValidID(order.UID))
                    OrderStrings.Insert(0, order.UID);
                else
                    _orders.Remove(order);
            }
            //_orders.Clear();
        }
        
        public void DeserializeMeals(FoodCollection foodCollection)
        {
            if (Type != LevelType.Fixed)
                return;

            _orders = new List<CustomerOrderPreset>();
            for (int i = OrderStrings.Count - 1; i >= 0; i--)
            {
                string id = OrderStrings[i];
                var preset = foodCollection.GetPreset(id);
                if (preset != null)
                    _orders.Insert(0, preset);
                else
                {
                    Debug.LogError($"Broken ID found: {id}");
                    OrderStrings.Remove(id);
                }
            }
            OrderStrings.Clear();
        }

        public ReadOnlyCollection<CustomerOrderPreset> GenerateOrders(FoodCollection foodCollection)
        {
            if (Type == LevelType.Fixed)
                return _orders.AsReadOnly();
            
            var presets = new List<CustomerOrderPreset>();
            int customersLeft = CustomersCount;
            int mealsLeft = TotalMealsNumber;
            
            while (mealsLeft > 0)
            {
                int numberOfMeals = MagicSelector(customersLeft, mealsLeft, MaxMealsInOneOrder);
                presets.Add(foodCollection.SelectRandomPreset(numberOfMeals));
                customersLeft--;
                mealsLeft--;
            }

            return presets.AsReadOnly();
        }

        //this is a really overcomplicated problem created by flawed design, so have this:
        private int MagicSelector(int customersLeft, int mealsLeft, int orderMealCap)
        {
            int maxMeals = (customersLeft - 1) * orderMealCap;
            for (int mealsInThisOrder = orderMealCap; mealsInThisOrder >= 1; mealsInThisOrder--)
            {
                if (maxMeals + mealsInThisOrder <= mealsLeft)
                    return Random.Range(mealsInThisOrder, MaxMealsInOneOrder);
            }
            return Random.Range(1, MaxMealsInOneOrder);
        }
        

        public enum LevelType
        {
            Random,
            Fixed,
        }
    }


}