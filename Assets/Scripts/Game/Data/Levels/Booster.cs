using System;
using GUI;
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
        
        public Booster(LevelManager levelManager, OrderManager orderManager, LevelGUI levelGUI)
        {
            _boostsNumber = new ReactiveProperty<int>();
            _boostsNumber.Subscribe(levelGUI.UpdateNumberOfBoosts);
            _orderManager = orderManager;
            _orderManager.OnOrderBoosted += OnBoostActionCompleted;
            levelManager.OnLevelStarted += StartLevel;
            
            levelGUI.OnGetBoostClick += GetNewBoost;
            levelGUI.OnSpendBoostClick += TrySpendBoost;
        }

        public void Initialize() { }

        private void StartLevel(EntryData data)
        {
            _boostsNumber.Value = data.NumberOfBoosts;
        }

        private void GetNewBoost() { _boostsNumber.Value += 1; }

        private void TrySpendBoost()
        {
            if (_boostsNumber.Value <= 0) return;
            _orderManager.TryCompleteOldestOrder();
        }

        private void OnBoostActionCompleted() { _boostsNumber.Value -= 1; }
    }
}