using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Food/Meal")]
    public class MealPreset : ScriptableObject
    {
        public string DisplayName = "M";
        [SerializeField] private string _uid;
        public string UID => _uid;
    }
}