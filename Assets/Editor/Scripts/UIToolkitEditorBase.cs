using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour))]
public class UIToolkitEditorBase : Editor
{
    public VisualTreeAsset InspectorXML;
    protected VisualElement _inspector;

    public override VisualElement CreateInspectorGUI()
    {
        _inspector = new VisualElement();
        if (InspectorXML == null)
            InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/LevelEditor.uxml");
        InspectorXML.CloneTree(_inspector);

        return _inspector;
    }

#endif
}