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
    public class LevelManager : IInitializable
    {
        private readonly Booster _booster;
        private readonly CustomerCounter _customerCounter;
        private readonly Action _onLevelOver;
        private readonly Settings _settings;
        private readonly Timer _timer;
        private readonly Action _onLevelCompleted;
        private readonly Action _onLevelFailed;
        private readonly Action<LevelData> _onLevelStarted;

        public LevelManager(GameController gameController, MainMenuGUI menuGUI, LevelGUI levelGUI, Restaurant restaurant, Settings settings)
        {
            _settings = settings;
            _customerCounter = new CustomerCounter(levelGUI.UpdateCustomersNumber, CompleteLevel);
            _timer = new Timer(gameController, levelGUI.UpdateLevelTimer, FailLevel);
            _booster = new Booster(levelGUI.UpdateNumberOfBoosts);
            _onLevelOver += _customerCounter.Cleanup;
            _onLevelOver += _booster.Cleanup;
            levelGUI.OnGetBoostClick += _booster.GetNewBoost;
            levelGUI.OnSpendBoostClick += _booster.TrySpendBoost;
            menuGUI.OnStart += () => StartLevel(restaurant);
            levelGUI.OnRestartClick += () => RestartLevel(restaurant);
            _onLevelCompleted += () => levelGUI.OpenMenu(LevelGUI.MenuMode.LevelCompleted);
            _onLevelFailed += () => levelGUI.OpenMenu(LevelGUI.MenuMode.LevelFailed);
            _onLevelStarted += restaurant.StartLevel;
        }
        
        public void Initialize() {  }
        
        private void StartLevel(Restaurant restaurant)
        {
            var levelData = new LevelData();
            if (!LevelEditor.GetLevelDataFromJSON(_settings.LevelFile, ref levelData, _settings.Food))
            {
                Debug.LogError("Error: couldn't read level data file.");
                return;
            }

            _booster.Init(levelData.NumberOfBoosts, restaurant);
            _timer.Init(levelData.TimeInSeconds);
            _customerCounter.Init(levelData.CustomersCount, restaurant);
            _onLevelStarted?.Invoke(levelData);
        }

        private void RestartLevel(Restaurant restaurant)
        {
            _onLevelOver?.Invoke();
            StartLevel(restaurant);
        }

        private void CompleteLevel()
        {
            _onLevelCompleted?.Invoke();
            _onLevelOver?.Invoke();
        }

        private void FailLevel()
        {
            _onLevelFailed?.Invoke();
            _onLevelOver?.Invoke();
        }

        [Serializable]
        public class Settings
        {
            public TextAsset LevelFile;
            public FoodCollection Food;
        }

        private class Timer
        {
            private readonly Action _timeOutEvent;
            private readonly ReactiveProperty<int> _timerValue;
            private float _timePassedSinceLastUpdate;

            public Timer(GameController gameController, Action<int> onTimerValueChanged, Action timeOutEvent)
            {
                _timerValue = new ReactiveProperty<int>();
                _timerValue.Subscribe(onTimerValueChanged);
                gameController.OnTimePassed += Update;
                _timeOutEvent = timeOutEvent;
            }

            public void Init(int timeInSeconds)
            {
                _timePassedSinceLastUpdate = 0f;
                _timerValue.Value = timeInSeconds;
            }

            private void Update(float deltaTime)
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

        private class Booster
        {
            private readonly ReactiveProperty<int> _boostsNumber;
            private Action _onBoostSpent;
            private Action _onCleanup;

            public Booster(Action<int> onValueChanged)
            {
                _boostsNumber = new ReactiveProperty<int>();
                _boostsNumber.Subscribe(onValueChanged);
            }

            public void Init(int value, Restaurant restaurant)
            {
                _onBoostSpent += restaurant.TryCompleteOldestOrder;
                restaurant.OnOrderBoosted += OnBoostActionCompleted;

                _onCleanup += () => _onBoostSpent -= restaurant.TryCompleteOldestOrder;
                _onCleanup += () => restaurant.OnOrderBoosted -= OnBoostActionCompleted;

                _boostsNumber.Value = value;
            }

            public void GetNewBoost() { _boostsNumber.Value += 1; }

            public void TrySpendBoost()
            {
                if (_boostsNumber.Value <= 0) return;
                _onBoostSpent?.Invoke();
            }

            private void OnBoostActionCompleted() { _boostsNumber.Value -= 1; }

            public void Cleanup()
            {
                _onCleanup?.Invoke();
                _onCleanup = null;
            }
        }

        private class CustomerCounter
        {
            private readonly Action _onComplete;
            private readonly ReactiveProperty<Vector2Int> _servedCustomers;
            private Action _onCleanup;

            public CustomerCounter(Action<Vector2Int> onValueChanged, Action onComplete)
            {
                _servedCustomers = new ReactiveProperty<Vector2Int>();
                _servedCustomers.Subscribe(onValueChanged);
                _onComplete = onComplete;
            }

            public void Init(int value, Restaurant restaurant)
            {
                _servedCustomers.Value = new Vector2Int(0, value);
                restaurant.OnCustomerServed += OnCustomerServed;
                _onCleanup += () => restaurant.OnCustomerServed -= OnCustomerServed;
            }

            private void OnCustomerServed()
            {
                _servedCustomers.Value = _servedCustomers.Value.ChangeX(1);
                if (_servedCustomers.Value.x == _servedCustomers.Value.y)
                    _onComplete?.Invoke();
            }

            public void Cleanup()
            {
                _onCleanup?.Invoke();
                _onCleanup = null;
            }
        }
    }
}