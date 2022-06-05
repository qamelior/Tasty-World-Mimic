using System;
using _Extensions;
using UnityEngine;
using Zenject;

namespace Restaurants.Customers
{
    public class CustomerSpot : MonoBehaviour
    {
        private Vector3 _spawnLocation;
        private Vector3 _destinationLocation;
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

        public void Setup(Vector3 from, Vector3 to, Action<CustomerSpot> onVacate)
        {
            _spawnLocation = from;
            _destinationLocation = to;
            _customersSpawnRoot.DestroyChildren();
            _orderGUI = GetComponent<GUI.CustomerOrderGUI>();
            _onSpotVacated += onVacate;
        }

        public void SpawnCustomer(string id, CustomerOrder order)
        {
            var c = _customerFactory.Create();
            c.transform.localPosition = _spawnLocation;
            c.Init(_destinationLocation, id, OnCustomerArrive);
            _order = order;
            _order.OnMealDelivered += _orderGUI.HideMeal;
            _order.OnOrderFulfilled += c.LeaveRestaurant;
            _order.OnOrderFulfilled += OnOrderFulfilled;
        }

        private void OnOrderFulfilled()
        {
            _orderGUI.HideAll();
            _onSpotVacated?.Invoke(this);
        }

        private void OnCustomerArrive() { _orderGUI.ShowAll(_order); }

        public class Factory : PlaceholderFactory<CustomerSpot>
        {
        }
    }
}
