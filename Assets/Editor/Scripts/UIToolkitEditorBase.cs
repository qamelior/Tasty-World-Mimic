using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour))]
public class UIToolkitEditorBase : Editor
{
    [SerializeField] private VisualTreeAsset _inspectorXML;
    protected VisualElement _inspector;

    public override VisualElement CreateInspectorGUI()
    {
        _inspector = new VisualElement();
        _inspectorXML.CloneTree(_inspector);

        return _inspector;
    }

#endif
}