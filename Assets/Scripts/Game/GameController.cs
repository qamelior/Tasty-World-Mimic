using System;
using Game.Data.Levels;
using GUI;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameController : ITickable
    {
        public const bool ShowDebugLogs = false;
        private GameStates _currentGameState;
        private Action<float> _onTimePassed;

        public GameController(MainMenuGUI menuUI, LevelGUI levelGUI)
        {
            _currentGameState = GameStates.MainMenu;
            menuUI.OnStartClick += () => ChangeGameState(GameStates.Playing);
            levelGUI.OnQuitClick += Quit;
            levelGUI.OnRestartClick += () => ChangeGameState(GameStates.Playing);
            levelGUI.OnMenuModeSwitch += mode =>
                ChangeGameState(mode == LevelGUI.MenuMode.Closed ? GameStates.Playing : GameStates.Menu);
        }

        public bool GameIsPaused => _currentGameState != GameStates.Playing;

        public void Tick()
        {
            if (_currentGameState == GameStates.Playing)
                _onTimePassed?.Invoke(Time.deltaTime);
        }

        public event Action<float> OnTimePassed { add => _onTimePassed += value; remove => _onTimePassed -= value; }


        public void Quit()
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