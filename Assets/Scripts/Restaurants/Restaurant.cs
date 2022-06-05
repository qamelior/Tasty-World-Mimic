using System;
using System.Collections;
using System.Collections.Generic;
using _Extensions;
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
        private Stack<CustomerOrder> _orders;
        private Stack<CustomerOrder> _activeOrders;
        private int _customerIDCounter;
        private Action _onCustomerServed;
        private CustomerOrder GetNextOrder() { return _orders.Count == 0 ? null : _orders.Pop(); }
        private readonly CustomerSpot.Factory _customerSpotFactory;
        private readonly Settings _settings;
        
        public Restaurant(CustomerSpot.Factory customerSpotFactory, MealSource.Factory mealSourceFactory, Settings settings)
        {
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
            _customerIDCounter = 1;
            CreateOrders();
            var customerSpots = CreateSpots();
            SpawnCustomers(customerSpots);
            _onCustomerServed = onCustomerServed;

            void CreateOrders()
            {
                _orders = new Stack<CustomerOrder>();
                var presets = levelData.GenerateOrders(_settings.ServedFood);
                foreach (var p in presets)
                    _orders.Push(new CustomerOrder(p));
            }

            List<CustomerSpot> CreateSpots()
            {
                var routes = _settings.CustomerRoutes;
                var spots = new List<CustomerSpot>();
                foreach (var route in routes)
                {
                    var spot = _customerSpotFactory.Create();
                    spot.Setup(route.SpawnPoint, route.DestinationPoint, TrySpawnNextCustomer);
                    spot.transform.localPosition = route.DestinationPoint;
                    spots.Add(spot);
                }
                return spots;
            }
 
            void SpawnCustomers(List<CustomerSpot> spots)
            {
                _activeOrders = new Stack<CustomerOrder>();
                foreach (var spot in spots)
                    TrySpawnNextCustomer(spot);
            }
        }

        private void TrySpawnNextCustomer(CustomerSpot spot)
        {
            var order = GetNextOrder();
            if (order == null)
                return;

            _activeOrders.Push(order);
            order.OnOrderFulfilled += _onCustomerServed;
            spot.SpawnCustomer($"{_customerIDCounter++}", order);
        }

        private void OnMealClicked(string mealID)
        {
            if (_activeOrders == null)
                return;
            foreach (var order in _activeOrders)
            {
                if (order.TryDeliverMeal(mealID))
                    break;
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
                public Vector3 SpawnPoint;
                public Vector3 DestinationPoint;
            }
        }
    }
}