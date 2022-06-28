using System;
using Game.Data;
using Restaurants.Customers.Orders;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Restaurants
{
    public class MealSource : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshPro _mealLabelRef;
        [SerializeField] private SpriteRenderer _sprite;
        private string _mealID;
        private Action<string> _onClick;
        public event Action<string> OnClick { add => _onClick += value; remove => _onClick -= value; } 
        private Settings _settings;

        public void OnPointerDown(PointerEventData data) { }
        public void OnPointerEnter(PointerEventData data) { _sprite.color = _settings.MouseOverColor; }
        public void OnPointerExit(PointerEventData data) { _sprite.color = _settings.DefaultColor; }
        public void OnPointerUp(PointerEventData data) { _onClick?.Invoke(_mealID); }

        [Inject]
        public void Construct(Settings settings)
        {
            _settings = settings;
        }

        public void Set(MealPreset mealPreset)
        {
            _mealID = mealPreset.UID;
            _mealLabelRef.text = mealPreset.DisplayName;
        }

        public class Factory : PlaceholderFactory<MealSource>
        {
        }

        [Serializable]
        public class Settings
        {
            public Color DefaultColor;
            public Color MouseOverColor;
        }
    }
}