using System;
using GUI;
using Restaurants;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameController : ITickable
    {
        public static bool ShowDebugLogs = true;
        private GameStates _currentGameState;
        private readonly Data.Levels.LevelManager _levelManager;
        private readonly Action<float> _gamePlayLoop;
        private readonly Restaurant _restaurant;

        public GameController(MainMenuGUI menuUI, LevelGUI levelGUI, Restaurant restaurant, Data.Levels.LevelManager levelManager)
        {
            _restaurant = restaurant;
            _levelManager = levelManager;
            _currentGameState = GameStates.MainMenu;
            _gamePlayLoop += levelManager.OnTimePassed;
            _levelManager.OnLevelCompleted += () => levelGUI.ToggleMenu(LevelGUI.MenuMode.LevelCompleted);
            _levelManager.OnLevelFailed += () => levelGUI.ToggleMenu(LevelGUI.MenuMode.LevelFailed);
            menuUI.OnStart += StartLevel;
            
            levelGUI.OnRestartClick += RestartLevel;
            levelGUI.OnQuitClick += Quit;
            levelGUI.OnMenuModeSwitch += (mode) =>
                ChangeGameState(mode == LevelGUI.MenuMode.Closed ? GameStates.Playing : GameStates.Menu);
        }

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

        private void RestartLevel()
        {
            // if (_currentGameState != GameStates.LevelFailed)
            //     return;
            // ChangeGameState(GameStates.Playing);
            // _levelManager.RestartLevel();
            StartLevel();
        }

        private void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
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
            Count,
        }
    }
}
