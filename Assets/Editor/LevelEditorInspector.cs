using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TastyWorld.Levels
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditorInspector : Editor
    {
        [SerializeField] private VisualTreeAsset _inspectorXML;
        private const string NewFileButtonID = "CreateNewLevel";
        private const string EditFileButtonID = "EditFileButton";
        private const string SaveFileButtonID = "SaveFileButton";
        private const string FakeFolderPathID = "LevelsFolderPath";
        private const string SelectedObjectFieldID = "SelectedFile";
        private const string SerializedLevelDataBlockID = "SelectedLevelInspector";
        private VisualElement _inspector;
        private LevelEditor _levelEditor;


        public override VisualElement CreateInspectorGUI()
        {
            _levelEditor = (LevelEditor)target;
            _inspector = new VisualElement();
            _inspectorXML.CloneTree(_inspector);

            _levelEditor.ValidateEditMode();
            //create new file
            FindVisualElement<Button>(NewFileButtonID)?.RegisterCallback<ClickEvent>(CreateNewFile);
            FindVisualElement<ObjectField>(SelectedObjectFieldID)?.RegisterValueChangedCallback(OnSelectedAssetChanged);
            //save folder path
            FindVisualElement<TextField>(FakeFolderPathID)?.SetText(_levelEditor.FakePath);
            UpdateEditModeVisuals();
            
            return _inspector;
        }

        private T FindVisualElement<T>(string elementName) where T : VisualElement
        {
            var elem = _inspector.Q<T>(elementName);
            if (elem == null)
                Debug.LogError($"[Level editor inspector] Incorrect ID for UI element: {elementName}");
            return elem;
        }

        private void UpdateEditModeVisuals()
        {
            bool hasFileSelected = _levelEditor.SelectedFile != null;
            //edit button
            SetUpDynamicButton(EditFileButtonID, hasFileSelected && !_levelEditor.EditMode, EditSelectedFile);
            //save button
            SetUpDynamicButton(SaveFileButtonID, _levelEditor.EditMode, SaveSelectedFile);
            //edited file inspector
            var serializedLevelDataBlock = FindVisualElement<VisualElement>(SerializedLevelDataBlockID);
            if (serializedLevelDataBlock != null)
            {
                serializedLevelDataBlock.SetVisibility(hasFileSelected);
                if (hasFileSelected)
                    serializedLevelDataBlock.SetEnabled(_levelEditor.EditMode);
            }
        }

        private void SetUpDynamicButton(string elementName, bool isVisible, EventCallback<ClickEvent> clickEvent)
        {
            var button = FindVisualElement<Button>(elementName);
            if (button == null)
                return;
            button.SetVisibility(isVisible); 
            if (button.visible)
                button.RegisterCallback(clickEvent);
        }

        private void CreateNewFile(ClickEvent evt) { _levelEditor.CreateNewFile(); }

        private void EditSelectedFile(ClickEvent evt)
        {
            _levelEditor.StartEdit();
            UpdateEditModeVisuals();
        }

        private void SaveSelectedFile(ClickEvent evt)
        {
            _levelEditor.FinishEdit();
            UpdateEditModeVisuals();
        }

        private void OnSelectedAssetChanged(ChangeEvent<Object> evt)
        {
            _levelEditor.OnSelectedAssetChanged();
            UpdateEditModeVisuals();
        }

    }
#endif
}