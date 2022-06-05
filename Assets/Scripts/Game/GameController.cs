using System;
using Restaurants;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameController : ITickable
    {
        private GameStates _currentGameState;
        private readonly Data.Levels.LevelManager _levelManager;
        private readonly Action<float> _gamePlayLoop;
        private readonly Restaurant _restaurant;

        public GameController(GUI.MainMenuGUI menuUI, Restaurant restaurant, Data.Levels.LevelManager levelManager)
        {
            _restaurant = restaurant;
            _levelManager = levelManager;
            _currentGameState = GameStates.Menu;
            _gamePlayLoop += levelManager.OnTimePassed;
            menuUI.OnStartButtonClicked += StartLevel;
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

        public void RestartLevel()
        {
            if (_currentGameState != GameStates.LevelFailed)
                return;
            ChangeGameState(GameStates.Playing);
            _levelManager.RestartLevel();
        }

        private void ChangeGameState(GameStates newState)
        {
            Debug.Log($"Game state: {newState}");
            _currentGameState = newState;
        }

        private enum GameStates
        {
            Menu,
            Playing,
            LevelCompleted,
            LevelFailed,
            BoosterShop,
            Count,
        }
    }
}
