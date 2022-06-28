using System;
using GUI;
using Restaurants;
using Restaurants.Customers.Orders;
using UnityEngine;
using Zenject;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// No LINQ, thank you

namespace Game.Data.Levels
{
    public class LevelManager
    {
        private readonly Action _onLevelCompleted;
        private readonly Action _onLevelFailed;
        private readonly Settings _settings;
        private Action _onLevelFinished;
        private Action<EntryData> _onLevelStarted;
        private Restaurant _selectedRestaurant;
        private int _customerUID;
        public LevelManager(MainMenuGUI menuGUI, LevelGUI levelGUI, Settings settings)
        {
            _settings = settings;
            _onLevelCompleted += () => levelGUI.OpenMenu(LevelGUI.MenuMode.LevelCompleted);
            _onLevelFailed += () => levelGUI.OpenMenu(LevelGUI.MenuMode.LevelFailed);
            levelGUI.OnRestartClick += RestartLevel;
            menuGUI.OnStartClick += StartLevel;
        }

        public event Action<EntryData> OnLevelStarted { add => _onLevelStarted += value; remove => _onLevelStarted -= value; }

        public void ChangeSelectedRestaurant(Restaurant restaurant) { _selectedRestaurant = restaurant; }

        private void StartLevel()
        {
            var levelData = new EntryData();
            if (!EntryEditor.GetLevelDataFromJSON(_settings.LevelFile, ref levelData, _settings.Food))
            {
                Debug.LogError("Error: couldn't read level data file.");
                return;
            }

            _customerUID = 1;
            _onLevelStarted?.Invoke(levelData);
            _selectedRestaurant.StartLevel();
        }

        public string GetCustomerUID() { return $"{_customerUID++}";}

        private void RestartLevel()
        {
            _onLevelFinished?.Invoke();
            StartLevel();
        }

        public void CompleteLevel()
        {
            _onLevelCompleted?.Invoke();
            _onLevelFinished?.Invoke();
        }

        public void FailLevel()
        {
            _onLevelFailed?.Invoke();
            _onLevelFinished?.Invoke();
        }

        [Serializable]
        public class Settings
        {
            public TextAsset LevelFile;
            public FoodCollection Food;
        }
    }
}