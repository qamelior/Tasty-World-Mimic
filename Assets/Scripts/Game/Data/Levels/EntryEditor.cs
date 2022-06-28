using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Game.Data.Levels
{
    public class EntryEditor : MonoBehaviour
    {
        private const string LevelsFolderPath = "LevelData/";
        private const string FileName = "TastyLevel_";
        [SerializeField] private FoodCollection _foodCollection;
        [SerializeField] private TextAsset _selectedFile;
        [SerializeField] private EntryData _entryData;
        public bool EditMode { private set; get; }
        public TextAsset SelectedFile => _selectedFile;
        public string FakePath => $"Resources/{LevelsFolderPath}";
        private string ResourcesPath(string fileName) { return $"{LevelsFolderPath}{fileName}"; }
        private string FullFilePath(string fileName) { return $"{Application.dataPath}/Resources/{ResourcesPath(fileName)}.json"; }

        public void CreateNewFile(bool editFile = true, bool selectFile = true)
        {
            string uniqueName = PickFileName();
            _entryData = new EntryData();
            WriteJSONFile(uniqueName);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            if (selectFile || editFile)
                _selectedFile = Resources.Load<TextAsset>(ResourcesPath(uniqueName));

            if (editFile)
                StartEdit();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private string PickFileName()
        {
            var i = 1;
            while (i < int.MaxValue)
            {
                var uniqueName = $"{FileName}{i}";
                if (!File.Exists(FullFilePath(uniqueName)))
                    return uniqueName;
                i++;
            }

            return "!UpdateFilePickAlgorithm";
        }

        public void StartEdit() { EditMode = true; }

        public void FinishEdit(bool saveFile = true)
        {
            if (saveFile)
            {
                WriteJSONFile(_selectedFile.name);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }

            EditMode = false;
        }

        public void ValidateEditMode()
        {
            if (EditMode && _selectedFile == null)
                EditMode = false;
        }

        public void OnSelectedAssetChanged()
        {
            FinishEdit(false);
            GetLevelDataFromJSON(_selectedFile, ref _entryData, _foodCollection);
        }

        private void WriteJSONFile(string fileShortName)
        {
            _entryData.SerializeMeals(_foodCollection);
            _entryData.Validate();
            using var fs = File.Open(FullFilePath(fileShortName), FileMode.Create);
            using var writer = new StreamWriter(fs);
            writer.Write(JsonConvert.SerializeObject(_entryData));
        }

        public static bool GetLevelDataFromJSON(TextAsset file, ref EntryData data, FoodCollection foodCollection)
        {
            if (file == null) return false;

            data = JsonConvert.DeserializeObject<EntryData>(file.text);
            data.DeserializeMeals(foodCollection);
            return true;
        }

        private bool GetLevelDataFromJSON(string path)
        {
            var file = Resources.Load<TextAsset>(path);
            if (file == null)
                return false;
            _entryData = JsonConvert.DeserializeObject<EntryData>(file.text);
            return true;
        }
    }
}