using System;
using GUI;
using GUI.Level;
using Restaurants;
using Restaurants.Customers.Orders;
using UniRx;
using Zenject;

namespace Game.Data.Levels
{
    public class Booster: IInitializable
    {
        private readonly ReactiveProperty<int> _boostsNumber;
        private readonly OrderManager _orderManager;
        
        public Booster(LevelManager levelManager, OrderManager orderManager, MenuGUI menuGUI, BoostsGUI boostsGUI)
        {
            _boostsNumber = new ReactiveProperty<int>();
            _boostsNumber.Subscribe(boostsGUI.UpdateNumberOfBoosts);
            _orderManager = orderManager;
            _orderManager.OnOrderBoosted += OnBoostActionCompleted;
            levelManager.OnLevelStarted += StartLevel;
            
            menuGUI.OnGetBoostClick += GetNewBoost;
            boostsGUI.OnSpendBoostClick += TrySpendBoost;
        }

        public void Initialize() { }

        private void StartLevel(LevelDataEntry data)
        {
            _boostsNumber.Value = data.NumberOfBoosts;
        }

        private void GetNewBoost() { _boostsNumber.Value += 1; }

        private void TrySpendBoost()
        {
            if (_boostsNumber.Value <= 0) return;
            _orderManager.CompleteOldestOrder();
        }

        private void OnBoostActionCompleted() { _boostsNumber.Value -= 1; }
    }
}