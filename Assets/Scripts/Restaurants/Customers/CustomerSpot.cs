using System;
using _Extensions;
using Game;
using GUI;
using UnityEngine;
using Zenject;

namespace Restaurants.Customers
{
    public class CustomerSpot : MonoBehaviour
    {
        [SerializeField] private Transform _customersSpawnRoot;
        private Customer.Factory _customerFactory;
        private Vector3 _customerSpawnLocation;
        private Action<CustomerSpot> _onSpotVacated;
        private CustomerOrder _order;
        private CustomerOrderGUI _orderGUI;
        public Transform CustomerSpawnRoot => _customersSpawnRoot;

        [Inject]
        public void Construct(Customer.Factory customerFactory) { _customerFactory = customerFactory; }

        public void Setup(Vector3 customerSpawnLocation, Action<CustomerSpot> onVacate)
        {
            _customerSpawnLocation = customerSpawnLocation;
            _customersSpawnRoot.DestroyChildren();
            _orderGUI = GetComponent<CustomerOrderGUI>();
            _orderGUI.Init();
            _orderGUI.HideAll();
            _onSpotVacated += onVacate;
        }

        public void SpawnCustomer(string id, CustomerOrder order, Action onArrive)
        {
            var customer = _customerFactory.Create();

            var customerTransform = customer.transform;
            customerTransform.SetParent(_customersSpawnRoot);
            customerTransform.position = _customerSpawnLocation;

            if (GameController.ShowDebugLogs)
                Debug.LogFormat($"Spawning customer {id} with order: {order.Preset.UID}");
            customer.Init(transform.position, id);
            customer.OnArrive += onArrive;
            customer.OnArrive += () => _orderGUI.ShowAll(_order);
            _order = order;
            _order.OnMealDelivered += _orderGUI.HideMeal;
            _order.OnOrderFulfilled += customer.MoveToExit;
            _order.OnOrderFulfilled += () => _orderGUI.HideAll();
            _order.OnOrderFulfilled += () => _onSpotVacated?.Invoke(this);
        }

        public class Factory : PlaceholderFactory<CustomerSpot>
        {
        }
    }
}