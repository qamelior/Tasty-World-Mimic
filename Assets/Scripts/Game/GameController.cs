using System;
using GUI;
using GUI.Level;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class GameController
    {
        public enum GameStates
        {
            MainMenu,
            Playing,
            Menu,
            Count
        }

        public const bool ShowDebugLogs = true;
        private readonly ReactiveProperty<GameStates> _state;

        public GameController(MainMenuGUI mainMenuUI, MenuGUI menuGUI)
        {
            (_state = new ReactiveProperty<GameStates>()).Value = GameStates.MainMenu;
            mainMenuUI.OnStartClick += () => ChangeGameState(GameStates.Playing);
            menuGUI.OnQuitClick += Quit;
            menuGUI.OnRestartClick += () => ChangeGameState(GameStates.Playing);
            menuGUI.OnMenuModeSwitch += mode =>
                ChangeGameState(mode == MenuGUI.MenuMode.Closed ? GameStates.Playing : GameStates.Menu);
        }

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
            _state.Value = newState;
        }

        public void SubscribeToGameStateChange(Action<GameStates> evt)
        {
            _state.Subscribe(evt);
        }
    }
}