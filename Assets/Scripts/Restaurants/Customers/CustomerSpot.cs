using System;
using _Extensions;
using Game;
using Game.Data.Levels;
using GUI;
using Restaurants.Customers.Orders;
using UnityEngine;
using Zenject;

namespace Restaurants.Customers
{
    public class CustomerSpot : MonoBehaviour
    {
        [SerializeField] private Transform _customersSpawnRoot;
        private Customer.Factory _customerFactory;
        private Vector3 _customerSpawnLocation;
        private CustomerOrderGUI _orderGUI;
        private OrderManager _orderLevelManager;
        private LevelManager _levelLevelManager;

        [Inject]
        public void Construct(Restaurant restaurant, LevelManager levelManager, OrderManager orderManager, Customer.Factory customerFactory)
        {
            _levelLevelManager = levelManager;
            _orderLevelManager = orderManager;
            _customerFactory = customerFactory;
        }

        public void Init(Vector3 position, Vector3 customerSpawnLocation)
        {
            transform.position = position;
            _customerSpawnLocation = customerSpawnLocation;
            _customersSpawnRoot.DestroyChildren();
            _orderGUI = GetComponent<CustomerOrderGUI>();
            _orderGUI.Init();

            TrySpawnCustomer();
        }

        private void TrySpawnCustomer()
        {
            var order = _orderLevelManager.PopNextOrder(_orderGUI);
            if (order == null)
                return;

            string id = _levelLevelManager.GetCustomerUID();
            if (GameController.ShowDebugLogs)
                Debug.LogFormat($"Spawning customer {id} with order: {order.PresetSO.UID}");

            void OnArriveEvent() => _orderLevelManager.ActivateOrder(order);
            var customer = _customerFactory.Create();
            customer.Init(id, _customersSpawnRoot, _customerSpawnLocation, transform.position, OnArriveEvent);
            order.OnOrderFulfilled += customer.MoveToExit;
            order.OnOrderFulfilled += TrySpawnCustomer;
        }

        public class Factory : PlaceholderFactory<CustomerSpot>
        {
        }
    }
}