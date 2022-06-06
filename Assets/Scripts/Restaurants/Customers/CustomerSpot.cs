using System;
using _Extensions;
using Game;
using UnityEngine;
using Zenject;

namespace Restaurants.Customers
{
    public class CustomerSpot : MonoBehaviour
    {
        private Vector3 _customerSpawnLocation;
        [SerializeField] private Transform _customersSpawnRoot;
        public Transform CustomerSpawnRoot => _customersSpawnRoot;
        private CustomerOrder _order;
        private GUI.CustomerOrderGUI _orderGUI;
        private Action<CustomerSpot> _onSpotVacated;
        private Customer.Factory _customerFactory;


        [Inject]
        public void Construct(Customer.Factory customerFactory)
        {
            _customerFactory = customerFactory;
        }

        public void Setup(Vector3 customerSpawnLocation, Action<CustomerSpot> onVacate)
        {
            _customerSpawnLocation = customerSpawnLocation;
            _customersSpawnRoot.DestroyChildren();
            _orderGUI = GetComponent<GUI.CustomerOrderGUI>();
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
            _order.OnOrderFulfilled += () =>_orderGUI.HideAll();
            _order.OnOrderFulfilled += () => _onSpotVacated?.Invoke(this);
        }

        public class Factory : PlaceholderFactory<CustomerSpot>
        {
        }
    }
}
