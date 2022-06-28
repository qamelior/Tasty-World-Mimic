using System;
using GUI.Level;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Data.Levels
{
    public class Timer: IInitializable, ITickable
    {
        private readonly Action _timeOutEvent;
        private readonly ReactiveProperty<int> _timerValue;
        private float _timePassedSinceLastUpdate;
        private bool _isEnabled;

        public Timer(GameController gameController, LevelManager levelManager, TimerGUI timerGUI)
        {
            _isEnabled = false;
            _timerValue = new ReactiveProperty<int>();
            _timerValue.Subscribe(timerGUI.UpdateLevelTimer);
            _timeOutEvent += levelManager.FailLevel;
            gameController.SubscribeToGameStateChange(val => _isEnabled = val == GameController.GameStates.Playing);
            levelManager.OnLevelStarted += StartLevel;
        }

        public void Initialize() {  }
        
        private void StartLevel(LevelDataEntry data)
        {
            _timePassedSinceLastUpdate = 0f;
            _timerValue.Value = data.TimeInSeconds;
        }
        
        public void Tick()
        {
            if (!_isEnabled) return;
            _timePassedSinceLastUpdate += Mathf.Clamp(Time.deltaTime, 0f, Time.maximumDeltaTime);
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
}