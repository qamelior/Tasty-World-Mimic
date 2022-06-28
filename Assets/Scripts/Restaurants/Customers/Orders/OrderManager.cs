using System;
using System.Collections.Generic;
using Game;
using Game.Data;
using Game.Data.Levels;
using GUI;
using UnityEngine;

namespace Restaurants.Customers.Orders
{
    public class OrderManager
    {
        private readonly CustomerCounter _customerCounter;
        private readonly Settings _settings;
        private List<Order> _activeOrders;
        private Action _onOrderBoosted;
        private Queue<Order> _orders;

        public OrderManager(LevelManager levelManager, CustomerCounter customerCounter, Settings settings)
        {
            _settings = settings;
            levelManager.OnLevelStarted += CreateOrders;
            _customerCounter = customerCounter;
        }

        public Order PopNextOrder(CustomerOrderGUI orderGUI)
        {
            if (_orders.Count == 0)
                return null;

            var order = _orders.Dequeue();
            order.Init(orderGUI);

            order.OnOrderFulfilled += () => _activeOrders.Remove(order);
            order.OnOrderFulfilled += _customerCounter.SubtractOneCustomer;
            return order;
        }

        public void ActivateOrder(Order order)
        {
            _activeOrders.Add(order);
            order.Activate();
        }

        public event Action OnOrderBoosted { add => _onOrderBoosted += value; remove => _onOrderBoosted -= value; }

        private void CreateOrders(LevelDataEntry entryData)
        {
            _orders = new Queue<Order>();
            _activeOrders = new List<Order>();
            var presets = entryData.GenerateOrders(_settings.ServedFood);
            foreach (var p in presets)
                _orders.Enqueue(new Order(p));
            if (GameController.ShowDebugLogs)
                Debug.Log($"This level has {_orders.Count} orders");
        }

        public void CompleteOldestOrder()
        {
            if (_activeOrders == null || _activeOrders.Count < 1)
                return;
            var order = _activeOrders[0];
            order.ForceComplete();
            _onOrderBoosted?.Invoke();
        }

        public void DeliverMealToOrder(string mealID)
        {
            if (_activeOrders == null)
                return;
            if (GameController.ShowDebugLogs)
                Debug.Log($"Trying to deliver {mealID}.\nActive orders count:{_activeOrders.Count}");
            foreach (var order in _activeOrders)
                if (order.TryDeliverMeal(mealID))
                {
                    if (GameController.ShowDebugLogs)
                        Debug.Log($"Delivered {mealID} to {order.PresetSO}");
                    break;
                }
        }

        [Serializable]
        public class Settings
        {
            public FoodCollection ServedFood;
        }
    }
}