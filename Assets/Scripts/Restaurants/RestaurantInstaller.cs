using System;
using _Extensions;
using Restaurants.Customers;
using Restaurants.Customers.Orders;
using UnityEngine;
using Zenject;

namespace Restaurants
{
    public class RestaurantInstaller : MonoInstaller
    {
        [SerializeField] private Transform _mealSourcesHolder;
        [SerializeField] private Transform _customerSpotHolder;
        [Inject] private FactoryPrefabs _prefabs;

        public override void InstallBindings()
        {
            ClearSpawnTransforms();
            Container.Bind<OrderManager>().AsSingle();
            Container.Bind<Restaurant>().AsSingle().WithArguments(_customerSpotHolder);
            Container.BindFactory<Customer, Customer.Factory>().FromComponentInNewPrefab(_prefabs.Customer)
                .WithGameObjectName("Customer");

            Container.BindFactory<MealSource, MealSource.Factory>().FromComponentInNewPrefab(_prefabs.MealSource)
                .WithGameObjectName("MealSource").UnderTransform(_mealSourcesHolder);

            Container.BindFactory<CustomerSpot, CustomerSpot.Factory>().FromComponentInNewPrefab(_prefabs.CustomerSpot)
                .WithGameObjectName("CustomerSpot").UnderTransform(_customerSpotHolder);
        }


        private void ClearSpawnTransforms()
        {
            _mealSourcesHolder.DestroyChildren();
            _customerSpotHolder.DestroyChildren();
        }

        [Serializable]
        public class FactoryPrefabs
        {
            public GameObject MealSource;
            public GameObject CustomerSpot;
            public GameObject Customer;
        }
    }
}