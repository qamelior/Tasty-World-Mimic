using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace GUI.Level
{
    public class TimerGUI : GUIWindow
    {
        [SerializeField] private string _levelTimerID = "LevelTimer";
        private Label _levelTimer;

        [Inject]
        public override void Construct()
        {
            base.Construct();
            _levelTimer = _root.FindVisualElement<Label>(_levelTimerID);
        }

        public void UpdateLevelTimer(int timeLeftSeconds)
        {
            int minutes = timeLeftSeconds / 60;
            int seconds = timeLeftSeconds % 60;
            _levelTimer.text = $"{minutes:00}:{seconds:00}";
        }
    }
}