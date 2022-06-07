using _Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUI;

[RequireComponent(typeof(UIDocument))]
public class MovableWindowGUI : MonoBehaviour
{
    [SerializeField] private Transform _anchor;
    [SerializeField] private string _rootElementID = "Root";
    private VisualElement _rootPanel;

    private void Awake()
    {
        var document = GetComponent<UIDocument>();
        _rootPanel = document.rootVisualElement.Q<VisualElement>(_rootElementID);
    }

    private void Update() { UpdateWindowPosition(); }

    private void UpdateWindowPosition() { _rootPanel.SetWorldPosition(_anchor.position); }
}