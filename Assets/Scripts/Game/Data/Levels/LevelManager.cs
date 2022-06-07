using System;
using _Extensions;
using GUI;
using Restaurants;
using UniRx;
using UnityEngine;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// No LINQ, thank you

namespace Game.Data.Levels
{
    public class LevelManager
    {
        private readonly Settings _settings;
        private ReactiveProperty<int> _boostsNumber;
        private LevelData _levelData;
        private Action _onBoostClicked;
        private Action _onLevelCompleted;
        private Action _onLevelFailed;
        private ReactiveProperty<Vector2Int> _servedCustomers;
        private LevelTimer _timer;

        public LevelManager(LevelGUI levelGUI, Settings settings)
        {
            _settings = settings;
            InitCustomersCounter();
            InitTimer();
            InitBoosts();

            void InitCustomersCounter()
            {
                _servedCustomers = new ReactiveProperty<Vector2Int>();
                _servedCustomers.Subscribe(levelGUI.UpdateCustomersNumber);
            }

            void InitTimer()
            {
                _timer = new LevelTimer();
                _timer.TimerValue.Subscribe(levelGUI.UpdateLevelTimer);
                _timer.TimeOutEvent += OnTimeOut;
            }

            void InitBoosts()
            {
                _boostsNumber = new ReactiveProperty<int>();
                _boostsNumber.Subscribe(levelGUI.UpdateNumberOfBoosts);
                levelGUI.OnGetBoostClick += GetBoost;
                levelGUI.OnSpendBoostClick += () => _onBoostClicked?.Invoke();
            }
        }

        public event Action OnLevelCompleted { add => _onLevelCompleted += value; remove => _onLevelCompleted -= value; }

        public event Action OnLevelFailed { add => _onLevelFailed += value; remove => _onLevelFailed -= value; }

        public void StartLevel(Restaurant restaurant)
        {
            LevelEditor.GetLevelDataFromJSON(_settings.LevelFile, ref _levelData, _settings.Food);
            _boostsNumber.Value = _levelData.NumberOfBoosts;
            _timer.Start(_levelData.TimeInSeconds);
            _servedCustomers.Value = new Vector2Int(0, _levelData.CustomersCount);

            restaurant.StartLevel(_levelData, OnCustomerServed);
            restaurant.OnOrderEnforced += SpendBoost;
            _onBoostClicked = restaurant.TryCompleteOldestOrder;
        }

        private void GetBoost() { _boostsNumber.Value += 1; }

        private void SpendBoost()
        {
            if (_boostsNumber.Value <= 0) return;
            _boostsNumber.Value -= 1;
        }

        public void OnTimePassed(float deltaTime) { _timer.Update(deltaTime); }

        private void OnTimeOut() { _onLevelFailed?.Invoke(); }

        private void OnCustomerServed()
        {
            _servedCustomers.Value = _servedCustomers.Value.ChangeX(1);
            if (_servedCustomers.Value.x == _servedCustomers.Value.y) _onLevelCompleted?.Invoke();
        }

        [Serializable]
        public class Settings
        {
            public TextAsset LevelFile;
            public FoodCollection Food;
        }

        private class LevelTimer
        {
            private Action _timeOutEvent;
            private float _timePassedSinceLastUpdate;

            public LevelTimer() { TimerValue = new ReactiveProperty<int>(); }
            public ReactiveProperty<int> TimerValue { get; }

            public event Action TimeOutEvent { add => _timeOutEvent += value; remove => _timeOutEvent -= value; }

            public void Start(int timeInSeconds)
            {
                _timePassedSinceLastUpdate = 0f;
                TimerValue.Value = timeInSeconds;
            }

            public void Update(float deltaTime)
            {
                _timePassedSinceLastUpdate += deltaTime;
                while (_timePassedSinceLastUpdate >= 1f)
                {
                    _timePassedSinceLastUpdate -= 1f;
                    TimerValue.Value -= 1;
                    if (TimerValue.Value <= 0)
                    {
                        _timeOutEvent?.Invoke();
                        break;
                    }
                }
            }
        }
    }
}