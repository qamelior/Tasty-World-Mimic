using System;
using _Extensions;
using Game.Data;
using Game.Data.Levels;
using Restaurants.Customers;
using Restaurants.Customers.Orders;
using UnityEngine;
using Zenject;

namespace Restaurants
{
    public class Restaurant: IInitializable
    {
        private readonly Transform _customersHolder;
        private readonly CustomerSpot.Factory _customerSpotFactory;
        private readonly Settings _settings;

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
                source.Set(sourceData.MealType, orderManager.DeliverMealToOrder);
            }

            levelManager.OnLevelStartedDelayed += StartLevel;
        }

        public void Initialize() { }

        private void StartLevel(LevelDataEntry obj)
        {
            //mop the floor
            _customersHolder.DestroyChildren();

            //create spots
            foreach (var route in _settings.CustomerRoutes)
                _customerSpotFactory.Create().StartLevel(route.SpotLocation, route.CustomerSpawnLocation);
        }

        [Serializable]
        public class Settings
        {
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