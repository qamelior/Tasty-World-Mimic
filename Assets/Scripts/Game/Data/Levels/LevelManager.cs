using System;
using _Extensions;
using GUI;
using Restaurants;
using UniRx;
using UnityEngine;
using Zenject;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// No LINQ, thank you

namespace Game.Data.Levels
{
    public class LevelManager
    {
        private readonly LevelGUI _ui;
        private LevelData _levelData;
        private LevelTimer _timer;
        private ReactiveProperty<Vector2Int> _servedCustomers;
        private ReactiveProperty<int> _boostsNumber;
        private readonly Settings _settings;
        private Action _askToCompleteOrder;
        private Action _onLevelCompleted;
        private Action _onLevelFailed;
        public event Action OnLevelCompleted
        {
            add => _onLevelCompleted += value;
            remove => _onLevelCompleted -= value;
        }
        public event Action OnLevelFailed
        {
            add => _onLevelFailed += value;
            remove => _onLevelFailed -= value;
        }
        
        public LevelManager(LevelGUI levelGUI, Settings settings)
        {
            _settings = settings;
            _ui = levelGUI;
            InitCustomersCounter();
            InitTimer();
            InitBoosts();

            void InitCustomersCounter()
            {
                _servedCustomers = new ReactiveProperty<Vector2Int>();
                _servedCustomers.Subscribe(_ui.UpdateCustomersNumber);
            }
            void InitTimer()
            {
                _timer = new LevelTimer();
                _timer.TimerValue.Subscribe(_ui.UpdateLevelTimer);
                _timer.TimeOutEvent += OnTimeOut;
            }
            void InitBoosts()
            {
                _boostsNumber = new ReactiveProperty<int>();
                _boostsNumber.Subscribe(_ui.UpdateNumberOfBoosts);
                _ui.OnGetBoostClick += GetBoost;
                _ui.OnSpendBoostClick += OnBoostClicked;
            }
        }

        public void StartLevel(Restaurant restaurant)
        {
            LevelEditor.GetLevelDataFromJSON(_settings.LevelFile, ref _levelData, _settings.Food);
            _boostsNumber.Value = _levelData.NumberOfBoosts;
            _timer.Start(_levelData.TimeInSeconds);
            _servedCustomers.Value = new Vector2Int(0, _levelData.CustomersCount);
            
            restaurant.StartLevel(_levelData, OnCustomerServed);
            restaurant.OnOrderEnforced += SpendBoost;
            _askToCompleteOrder = restaurant.TryCompleteOldestOrder;

        }

        private void GetBoost() { _boostsNumber.Value += 1; }
        private void SpendBoost()
        {
            if (_boostsNumber.Value <= 0) return;
            _boostsNumber.Value -= 1;
        }

        private void OnBoostClicked() { _askToCompleteOrder?.Invoke(); }

        public void OnTimePassed(float deltaTime) => _timer.Update(deltaTime);
        private void OnTimeOut() { _onLevelFailed?.Invoke(); }

        private void OnCustomerServed()
        {
            _servedCustomers.Value = _servedCustomers.Value.ChangeX(1);
            if (_servedCustomers.Value.x == _servedCustomers.Value.y)
            {
                _onLevelCompleted?.Invoke();
            }
        }

        [Serializable]
        public class Settings
        {
            public TextAsset LevelFile;
            public FoodCollection Food;
        }

        private class LevelTimer
        {
            private readonly ReactiveProperty<int> _timerValue;
            private float _timePassedSinceLastUpdate;
            private Action _timeOutEvent;
            public ReactiveProperty<int> TimerValue => _timerValue;
            public event Action TimeOutEvent
            {
                add => _timeOutEvent += value;
                remove => _timeOutEvent -= value;
            }

            public LevelTimer() { _timerValue = new ReactiveProperty<int>(); }

            public void Start(int timeInSeconds)
            {
                _timePassedSinceLastUpdate = 0f;
                _timerValue.Value = timeInSeconds;
            }
            
            public void Update(float deltaTime)
            {
                _timePassedSinceLastUpdate += deltaTime;
                while (_timePassedSinceLastUpdate >= 1f)
                {
                    _timePassedSinceLastUpdate -= 1f;
                    _timerValue.Value -= 1;
                    if (_timerValue.Value <= 0)
                    {
                        _timeOutEvent?.Invoke();
                        break;
                    }
                }
            }
        }
    }
}