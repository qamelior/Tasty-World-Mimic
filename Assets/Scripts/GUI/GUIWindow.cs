using UnityEngine;
using UnityEngine.UIElements;

namespace GUI
{
    [RequireComponent(typeof(UnityEngine.UIElements.UIDocument))]
    public class GUIWindow : MonoBehaviour
    {
        private UIDocument _ui;
        protected VisualElement _root;

        protected virtual void OnEnable()
        {
            _ui = GetComponent<UIDocument>();
            _root = _ui.rootVisualElement;
        }

    }
}
