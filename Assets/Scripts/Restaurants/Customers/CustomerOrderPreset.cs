using Game.Data;
using UnityEngine;

namespace Restaurants.Customers
{
    [CreateAssetMenu(menuName = "Food/Order Preset")]
    public class CustomerOrderPreset : ScriptableObject
    {
        [SerializeField] private string _uid;
        public string UID => _uid;
    
        [SerializeField] private MealPreset[] _meals;
        public MealPreset[] Meals => _meals;

        [ContextMenu("Set ID based on meals")]
        public void SetIDBasedOnContents()
        {
            if (_meals == null || _meals.Length < 1)
                return;

            _uid = "";
            foreach (var m in _meals)
            {
                _uid += m.name;
            }
        }
    }
}
