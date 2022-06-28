using System;
using System.Collections.Generic;
using _Extensions;
using Game;
using Game.Data;
using Game.Data.Levels;
using Restaurants.Customers;
using Restaurants.Customers.Orders;
using UnityEngine;

namespace Restaurants
{
    public class Restaurant
    {
        private readonly Transform _customersHolder;
        private readonly CustomerSpot.Factory _customerSpotFactory;
        private readonly Settings _settings;

        private int _customerIDCounter;
        private Action _onLevelStarted;

        public FoodCollection ServedFood => _settings.ServedFood;

        public Restaurant(LevelManager levelManager, OrderManager orderManager, Transform customersHolder,
            CustomerSpot.Factory customerSpotFactory, MealSource.Factory mealSourceFactory,
            Settings settings)
        {
            _customersHolder = customersHolder;
            _customerSpotFactory = customerSpotFactory;
            _settings = settings;

            foreach (var sourceData in _settings.MealSources)
            {
                var source = mealSourceFactory.Create();
                source.transform.localPosition = sourceData.LocalPosition;
                source.Set(sourceData.MealType);
                source.OnClick += orderManager.OnMealClicked;
            }

            levelManager.ChangeSelectedRestaurant(this);
            orderManager.ChangeSelectedRestaurant(this);
        }

        public void StartLevel()
        {
            //mop the floor
            _customersHolder.DestroyChildren();

            //create spots
            foreach (var route in _settings.CustomerRoutes)
                _customerSpotFactory.Create().Init(route.SpotLocation, route.CustomerSpawnLocation);
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