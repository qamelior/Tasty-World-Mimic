using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace TastyWorld.Levels
{
    public class LevelManager : MonoBehaviour
    {
        private float _levelTimer = 0f;
        private LevelData _levelData;

        public void UpdateLevelTimer(float deltaTime)
        {
            _levelTimer -= deltaTime;
            if (_levelTimer <= 0f)
            {
                _levelTimer = 0f;
            }
        }

        public void RestartLevel()
        {

        }
    }
}