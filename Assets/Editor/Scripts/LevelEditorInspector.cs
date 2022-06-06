using _Extensions;
using Game.Data;
using Game.Data.Levels;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelEditor))]
public class LevelEditorInspector : UIToolkitEditorBase
{
    private LevelEditor _editor;
    private const string NewFileButtonID = "CreateNewLevel";
    private const string EditFileButtonID = "EditFileButton";
    private const string SaveFileButtonID = "SaveFileButton";
    private const string FakeFolderPathID = "LevelsFolderPath";
    private const string SelectedObjectFieldID = "SelectedFile";
    private const string SerializedLevelDataBlockID = "SelectedLevelInspector";
    private const string MealCollectionSelectorID = "FoodCollectionSelector";

    public override VisualElement CreateInspectorGUI()
    {
        base.CreateInspectorGUI();
        _editor = (LevelEditor)target;
        _editor.ValidateEditMode();
        //create new file
        _inspector.FindVisualElement<Button>(NewFileButtonID)
            ?.RegisterCallback<ClickEvent>(evt => _editor.CreateNewFile());
        _inspector.FindVisualElement<ObjectField>(SelectedObjectFieldID)
            ?.RegisterValueChangedCallback(evt => OnSelectedAssetChanged());

        _inspector.FindVisualElement<ObjectField>(MealCollectionSelectorID)?.SetType(typeof(FoodCollection));
        
        //save folder path
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
