using System;
using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUI
{
    public class LevelGUI : GUIWindow
    {
        [Header("Element IDs")] [SerializeField]
        private string LevelTimerID = "LevelTimer";

        [SerializeField] private string CustomersNumberID = "CustomerCount";
        [SerializeField] private string LevelFinishedViewID = "LevelFinishedView";
        [SerializeField] private string LevelFinishedHeaderID = "LevelFinishedHeader";
        [SerializeField] private string RestartButtonID = "RestartButton";

        private Label _levelTimer;
        private Label _customersNumber;
        private VisualElement _levelFinishedView;
        private Label _levelFinishedHeader;
        private Action _restartLevelEvent;

        protected override void OnEnable()
        {
            base.OnEnable();
            _levelTimer = _root.FindVisualElement<Label>(LevelTimerID);
            _customersNumber = _root.FindVisualElement<Label>(CustomersNumberID);
            //level finished window
            _levelFinishedView = _root.FindVisualElement<VisualElement>(LevelFinishedViewID);
            _levelFinishedView.SetVisibility(false);
            _levelFinishedHeader = _root.FindVisualElement<Label>(LevelFinishedHeaderID);
            _root.FindVisualElement<Button>(RestartButtonID)?.RegisterCallback<ClickEvent>(evt => OnRestartButtonClicked());
        }

        public void UpdateLevelTimer(int timeLeftSeconds)
        {
            int minutes = timeLeftSeconds / 60;
            int seconds = timeLeftSeconds % 60;
            _levelTimer.text = $"{minutes:00}:{seconds:00}";
        }

        public void UpdateCustomersNumber(Vector2Int customersNumber)
        {
            _customersNumber.text = $"{customersNumber.x}/{customersNumber.y}";
        }

        private void OnRestartButtonClicked() { }

        private void OnLevelFinished(bool isCompleted) { _levelFinishedView.SetVisibility(true); }

    }
}
