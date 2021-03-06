using Game.Data;
using UnityEngine;

namespace Restaurants.Customers.Orders
{
    [CreateAssetMenu(menuName = "Food/Order Preset")]
    public class OrderPresetSO : ScriptableObject
    {
        [SerializeField] private string _uid;
        [SerializeField] private MealPreset[] _meals;
        public string UID => _uid;
        public MealPreset[] Meals => _meals;

        [ContextMenu("Set ID based on meals")]
        public void SetIDBasedOnContents()
        {
            if (_meals == null || _meals.Length < 1)
                return;

            _uid = "";
            foreach (var m in _meals) _uid += m.name;
        }
    }
}