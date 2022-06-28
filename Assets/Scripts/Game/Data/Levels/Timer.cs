using System;
using GUI.Level;
using UniRx;
using Zenject;

namespace Game.Data.Levels
{
    public class Timer: IInitializable
    {
        private readonly Action _timeOutEvent;
        private readonly ReactiveProperty<int> _timerValue;
        private float _timePassedSinceLastUpdate;

        public Timer(GameController gameController, LevelManager levelManager, TimerGUI timerGUI)
        {
            _timerValue = new ReactiveProperty<int>();
            _timerValue.Subscribe(timerGUI.UpdateLevelTimer);
            _timeOutEvent += levelManager.FailLevel;
            gameController.OnTimePassed += Update;
            levelManager.OnLevelStarted += StartLevel;
        }

        public void Initialize() {  }
        
        private void StartLevel(LevelDataEntry data)
        {
            _timePassedSinceLastUpdate = 0f;
            _timerValue.Value = data.TimeInSeconds;
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
}