using System;
using GUI;
using GUI.Level;
using UnityEngine;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// No LINQ, thank you

namespace Game.Data.Levels
{
    public class LevelManager
    {
        private readonly Action _onLevelCompleted;
        private readonly Action _onLevelFailed;
        private readonly Settings _settings;
        private Action<LevelDataEntry> _onLevelStarted;
        private Action<LevelDataEntry> _onLevelStartedDelayed;

        public LevelManager(MainMenuGUI mainMenuGUI, MenuGUI menuGUI, Settings settings)
        {
            _settings = settings;
            _onLevelCompleted += () => menuGUI.OpenMenu(MenuGUI.MenuMode.LevelCompleted);
            _onLevelFailed += () => menuGUI.OpenMenu(MenuGUI.MenuMode.LevelFailed);
            menuGUI.OnRestartClick += RestartLevel;
            mainMenuGUI.OnStartClick += StartLevel;
        }

        public event Action<LevelDataEntry> OnLevelStarted { add => _onLevelStarted += value; remove => _onLevelStarted -= value; }
        public event Action<LevelDataEntry> OnLevelStartedDelayed { add => _onLevelStartedDelayed += value; remove => _onLevelStartedDelayed -= value; }

        private void StartLevel()
        {
            var levelData = new LevelDataEntry();
            if (!EntryEditor.GetLevelDataFromJSON(_settings.LevelFile, ref levelData, _settings.Food))
            {
                Debug.LogError("Error: couldn't read level data file.");
                return;
            }

            _onLevelStarted?.Invoke(levelData);
            _onLevelStartedDelayed?.Invoke(levelData);
        }

        private void RestartLevel() { StartLevel(); }
        public void CompleteLevel() { _onLevelCompleted?.Invoke(); }
        public void FailLevel() { _onLevelFailed?.Invoke(); }

        [Serializable]
        public class Settings
        {
            public TextAsset LevelFile;
            public FoodCollection Food;
        }
    }
}