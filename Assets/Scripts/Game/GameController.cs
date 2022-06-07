using System;
using Game.Data.Levels;
using GUI;
using Restaurants;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameController : ITickable
    {
        public const bool ShowDebugLogs = true;
        private readonly Action<float> _gamePlayLoop;
        private readonly LevelManager _levelManager;
        private readonly Restaurant _restaurant;
        private GameStates _currentGameState;

        public GameController(MainMenuGUI menuUI, LevelGUI levelGUI, Restaurant restaurant, LevelManager levelManager)
        {
            _currentGameState = GameStates.MainMenu;
            _restaurant = restaurant;
            _levelManager = levelManager;
            _levelManager.OnLevelCompleted += () => levelGUI.ToggleMenu(LevelGUI.MenuMode.LevelCompleted);
            _levelManager.OnLevelFailed += () => levelGUI.ToggleMenu(LevelGUI.MenuMode.LevelFailed);
            _gamePlayLoop += levelManager.OnTimePassed;
            menuUI.OnStart += StartLevel;

            levelGUI.OnRestartClick += RestartLevel;
            levelGUI.OnQuitClick += Quit;
            levelGUI.OnMenuModeSwitch += mode =>
                ChangeGameState(mode == LevelGUI.MenuMode.Closed ? GameStates.Playing : GameStates.Menu);
        }

        public bool GameIsPaused => _currentGameState != GameStates.Playing;

        public void Tick()
        {
            if (_currentGameState == GameStates.Playing)
                _gamePlayLoop.Invoke(Time.deltaTime);
        }

        private void StartLevel()
        {
            ChangeGameState(GameStates.Playing);
            _levelManager.StartLevel(_restaurant);
        }

        private void RestartLevel() { StartLevel(); }

        private void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ChangeGameState(GameStates newState)
        {
            if (ShowDebugLogs)
                Debug.Log($"Game state: {newState}");
            _currentGameState = newState;
        }

        private enum GameStates
        {
            MainMenu,
            Playing,
            Menu,
            Count
        }
    }
}