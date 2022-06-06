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
        private readonly LevelTimer _timer;
        private ReactiveProperty<Vector2Int> _servedCustomers;
        private readonly Settings _settings;
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
            _servedCustomers = new ReactiveProperty<Vector2Int>();
            _ui = levelGUI;
            _timer = new LevelTimer(_ui.UpdateLevelTimer, _onLevelFailed);
            _servedCustomers.Subscribe(_ui.UpdateCustomersNumber);
        }

        public void StartLevel(Restaurant restaurant)
        {
            LevelEditor.GetLevelDataFromJSON(_settings.LevelFile, ref _levelData, _settings.Food);
            _timer.Start(_levelData.TimeInSeconds);
            _servedCustomers.Value = new Vector2Int(0, _levelData.CustomersCount);
            restaurant.StartLevel(_levelData, OnCustomerServed);
        }

        public void OnTimePassed(float deltaTime) => _timer.Update(deltaTime);

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
            private readonly ReactiveProperty<int> _levelTimer;
            private float _timePassedSinceLastUpdate;
            private readonly Action _timeOutEvent;
            
            public LevelTimer(Action<int> timeChangedEvent, Action timeOutEvent)
            {
                _levelTimer = new ReactiveProperty<int>();
                _levelTimer.Subscribe(timeChangedEvent);
                _timeOutEvent = timeOutEvent;
            }
            
            public void Start(int timeInSeconds)
            {
                _timePassedSinceLastUpdate = 0f;
                _levelTimer.Value = timeInSeconds;
            }
            
            public void Update(float deltaTime)
            {
                _timePassedSinceLastUpdate += deltaTime;
                while (_timePassedSinceLastUpdate >= 1f)
                {
                    _timePassedSinceLastUpdate -= 1f;
                    _levelTimer.Value -= 1;
                    if (_levelTimer.Value <= 0)
                    {
                        _timeOutEvent?.Invoke();
                        break;
                    }
                }
            }
        }
    }
}