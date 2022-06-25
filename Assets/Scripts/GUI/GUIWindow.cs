using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace GUI
{
    public class GUIWindow : MonoBehaviour
    {
        [SerializeField] private bool _enableOnStart = true;
        protected VisualElement _root;
        protected UIDocument _ui;

        [Inject]
        public virtual void Construct()
        {
            _ui = GetComponent<UIDocument>();
            _root = _ui.rootVisualElement;
            if (_enableOnStart)
                Show();
            else
                Hide();
        }

        protected void Show() { }

        protected void Hide() { }
    }
}