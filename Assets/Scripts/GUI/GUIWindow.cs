using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GUI
{
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
