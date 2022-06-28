using System;
using _Extensions;
using GUI;
using GUI.Level;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Data.Levels
{
    public class CustomerCounter: IInitializable
    {
        private readonly Action _onComplete;
        private readonly ReactiveProperty<Vector2Int> _servedCustomers;
        
        public CustomerCounter(LevelManager levelManager, CustomerCounterGUI customerCounterGUI)
        {
            _servedCustomers = new ReactiveProperty<Vector2Int>();
            _servedCustomers.Subscribe(customerCounterGUI.UpdateCustomersNumber);
            _onComplete = levelManager.CompleteLevel;
            levelManager.OnLevelStarted += StartLevel;
        }
        
        public void Initialize() {}

        private void StartLevel(LevelDataEntry data)
        {
            _servedCustomers.Value = new Vector2Int(0, data.CustomersCount);
        }

        public void SubtractOneCustomer()
        {
            _servedCustomers.Value = _servedCustomers.Value.ChangeX(1);
            if (_servedCustomers.Value.x == _servedCustomers.Value.y)
                _onComplete?.Invoke();
        }
    }
}