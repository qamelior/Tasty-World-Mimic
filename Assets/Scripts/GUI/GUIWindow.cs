using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Zenject;

namespace GUI
{
    public class GUIWindow : MonoBehaviour
    {
        protected UIDocument _ui;
        protected VisualElement _root;

        [Inject]
        public virtual void Construct()
        {
            _ui = GetComponent<UIDocument>();
            _root = _ui.rootVisualElement;
        }
    }
}
