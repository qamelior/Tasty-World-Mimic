using _Extensions;
using Game.Data;
using Game.Data.Levels;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(EntryEditor))]
public class LevelEditorInspector : Editor
{
    private EntryEditor _editor;
    private const string NewFileButtonID = "CreateNewLevel";
    private const string EditFileButtonID = "EditFileButton";
    private const string SaveFileButtonID = "SaveFileButton";
    private const string FakeFolderPathID = "LevelsFolderPath";
    private const string SelectedObjectFieldID = "SelectedFile";
    private const string SerializedLevelDataBlockID = "SelectedLevelInspector";
    private const string MealCollectionSelectorID = "FoodCollectionSelector";

    private VisualTreeAsset _inspectorXML;
    private VisualElement _inspector;

    public VisualElement CreateInspectorGUI()
    {
        _inspector = new VisualElement();
        if (_inspectorXML == null)
            _inspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/LevelEditor.uxml");
        _inspectorXML.CloneTree(_inspector);
        
        _editor = (EntryEditor)target;
        _editor.ValidateEditMode();

        _inspector.RegisterButtonEvent(NewFileButtonID, evt => _editor.CreateNewFile());
        _inspector.FindVisualElement<UnityEditor.UIElements.ObjectField>(SelectedObjectFieldID)
            ?.RegisterValueChangedCallback(evt => OnSelectedAssetChanged());
        _inspector.FindVisualElement<UnityEditor.UIElements.ObjectField>(MealCollectionSelectorID)?.SetType(typeof(FoodCollection));
        _inspector.FindVisualElement<TextField>(FakeFolderPathID)?.SetText(_editor.FakePath);
        
        UpdateEditModeVisuals();
        
        return _inspector;
    }

    private void UpdateEditModeVisuals()
    {
        bool hasFileSelected = _editor.SelectedFile != null;
        //edit button
        SetUpDynamicButton(EditFileButtonID, hasFileSelected && !_editor.EditMode, EditSelectedFile);
        //save button
        SetUpDynamicButton(SaveFileButtonID, _editor.EditMode, SaveSelectedFile);
        //edited file inspector
        var serializedLevelDataBlock = _inspector.FindVisualElement<VisualElement>(SerializedLevelDataBlockID);
        if (serializedLevelDataBlock != null)
        {
            serializedLevelDataBlock.SetVisibility(hasFileSelected);
            if (hasFileSelected)
                serializedLevelDataBlock.SetEnabled(_editor.EditMode);
        }
    }

    private void SetUpDynamicButton(string elementName, bool isVisible, EventCallback<ClickEvent> clickEvent)
    {
        var button = _inspector.FindVisualElement<Button>(elementName);
        if (button == null)
            return;
        button.SetVisibility(isVisible);
        if (button.visible)
            button.RegisterCallback(clickEvent);
    }

    private void EditSelectedFile(ClickEvent evt)
    {
        _editor.StartEdit();
        UpdateEditModeVisuals();
    }

    private void SaveSelectedFile(ClickEvent evt)
    {
        _editor.FinishEdit();
        UpdateEditModeVisuals();
    }

    private void OnSelectedAssetChanged()
    {
        _editor.OnSelectedAssetChanged();
        UpdateEditModeVisuals();
    }

}
#endif
