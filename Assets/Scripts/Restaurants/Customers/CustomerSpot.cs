using _Extensions;
using Game;
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
        private CustomerManager _customerManager;
        private Vector3 _customerSpawnLocation;
        private CustomerOrderGUI _orderGUI;
        private OrderManager _orderLevelManager;

        [Inject]
        public void Construct(CustomerManager customerManager, OrderManager orderManager, Customer.Factory customerFactory)
        {
            _customerManager = customerManager;
            _orderLevelManager = orderManager;
            _customerFactory = customerFactory;
        }

        public void StartLevel(Vector3 position, Vector3 customerSpawnLocation)
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

            string id = _customerManager.GetCustomerUID();
            if (GameController.ShowDebugLogs)
                Debug.LogFormat($"Spawning customer {id} with order: {order.PresetSO.UID}");
            
            void OnArriveEvent() { _orderLevelManager.ActivateOrder(order); }
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