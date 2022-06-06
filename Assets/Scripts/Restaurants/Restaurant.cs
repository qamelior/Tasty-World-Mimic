using System;
using System.Collections;
using System.Collections.Generic;
using _Extensions;
using Game;
using Game.Data;
using Game.Data.Levels;
using GUI;
using Restaurants.Customers;
using UnityEngine;
using Zenject;

namespace Restaurants
{
    public class Restaurant
    {
        private Queue<CustomerOrder> _orders;
        private List<CustomerOrder> _activeOrders;
        private int _customerIDCounter;
        private Action _onCustomerServed;
        private CustomerOrder GetNextOrder() { return _orders.Count == 0 ? null : _orders.Dequeue(); }
        private readonly CustomerSpot.Factory _customerSpotFactory;
        private readonly Settings _settings;
        private Transform _mealSourcesHolder;
        private Transform _customersHolder;

        private Action _onOrderEnforced;
        public event Action OnOrderEnforced
        {
            add => _onOrderEnforced += value;
            remove => _onOrderEnforced -= value;
        }
        
        public Restaurant(Transform mealSourcesHolder, Transform customersHolder,
            CustomerSpot.Factory customerSpotFactory, MealSource.Factory mealSourceFactory, Settings settings)
        {
            _mealSourcesHolder = mealSourcesHolder;
            _customersHolder = customersHolder;
            _customerSpotFactory = customerSpotFactory;
            _settings = settings;
            SpawnMealSources();

            void SpawnMealSources()
            {
                foreach (var sourceData in _settings.MealSources)
                {
                    var source = mealSourceFactory.Create();
                    source.transform.localPosition = sourceData.LocalPosition;
                    source.Set(sourceData.MealType, OnMealClicked);
                }
            }
        }

        public void StartLevel(LevelData levelData,  Action onCustomerServed)
        {
            MopTheFloor();
            _customerIDCounter = 1;
            CreateOrders();
            var customerSpots = CreateSpots();
            SpawnCustomers(customerSpots);
            _onCustomerServed = onCustomerServed;
            _onOrderEnforced = null;

            void CreateOrders()
            {
                _orders = new Queue<CustomerOrder>();
                _activeOrders = new List<CustomerOrder>();
                var presets = levelData.GenerateOrders(_settings.ServedFood);
                foreach (var p in presets)
                    _orders.Enqueue(new CustomerOrder(p));
                if (GameController.ShowDebugLogs)
                    Debug.Log($"This level has {_orders.Count} orders");
            }

            List<CustomerSpot> CreateSpots()
            {
                var routes = _settings.CustomerRoutes;
                var spots = new List<CustomerSpot>();
                foreach (var route in routes)
                {
                    var spot = _customerSpotFactory.Create();
                    spot.transform.position = route.SpotLocation;
                    spot.Setup(route.CustomerSpawnLocation, TrySpawnNextCustomer);
                    spots.Add(spot);
                }
                return spots;
            }
 
            void SpawnCustomers(List<CustomerSpot> spots)
            {
                foreach (var spot in spots)
                    TrySpawnNextCustomer(spot);
            }
        }

        public void TryCompleteOldestOrder()
        {
            if (_activeOrders == null || _activeOrders.Count < 1)
                return;
            var order = _activeOrders[0];
            order.ForceComplete();
            _onOrderEnforced?.Invoke();
        }

        private void MopTheFloor()
        {
            _customersHolder.DestroyChildren();
        }

        private void TrySpawnNextCustomer(CustomerSpot spot)
        {
            var order = GetNextOrder();
            if (order == null)
                return;

            order.OnOrderFulfilled += () => _onCustomerServed?.Invoke();
            order.OnOrderFulfilled += () => _activeOrders.Remove(order);
            spot.SpawnCustomer($"{_customerIDCounter++}", order, () => _activeOrders.Add(order));
        }

        private void OnMealClicked(string mealID)
        {
            if (_activeOrders == null)
                return;
            if (Game.GameController.ShowDebugLogs)
                Debug.Log($"Trying to deliver {mealID}.\nActive orders count:{_activeOrders.Count}");
            foreach (var order in _activeOrders)
            {
                if (order.TryDeliverMeal(mealID))
                {
                    if (Game.GameController.ShowDebugLogs)
                        Debug.Log($"Delivered {mealID} to {order.Preset}");
                    break;
                }
            }
        }


        [Serializable]
        public class Settings
        {
            public FoodCollection ServedFood;
            public MealSourceData[] MealSources;
            public CustomerRoute[] CustomerRoutes;
            
            [Serializable]
            public class MealSourceData
            {
                public MealPreset MealType;
                public Vector3 LocalPosition;
            }
            
            [Serializable]
            public class CustomerRoute
            {
                public Vector3 SpotLocation;
                public Vector3 CustomerSpawnLocation;
            }
        }
    }
}