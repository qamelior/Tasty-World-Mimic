using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Game.Data.Levels
{
    public class LevelEditor : MonoBehaviour
    {
        public bool EditMode { private set; get; } = false;
        
        private const string LevelsFolderPath = "LevelData/";
        private const string FileName = "TastyLevel_";

        [SerializeField] private FoodCollection _foodCollection;
        [SerializeField] private TextAsset _selectedFile;
        [SerializeField] private LevelData _levelData;
        public TextAsset SelectedFile => _selectedFile;

        //custom file paths
        public string FakePath => $"Resources/{LevelsFolderPath}";
        private string ResourcesPath(string fileName) { return $"{LevelsFolderPath}{fileName}"; }

        private string FullFilePath(string fileName)
        {
            return $"{Application.dataPath}/Resources/{ResourcesPath(fileName)}.json";
        }


        public void CreateNewFile(bool editFile = true, bool selectFile = true)
        {
            string uniqueName = PickFileName();
            _levelData = new LevelData(uniqueName);
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
        
        public void StartEdit()
        {
            // if (!ReadJSONFile())
            //     return;
            EditMode = true;
        }

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
            GetLevelDataFromJSON(_selectedFile, ref _levelData, _foodCollection);
        }
        
        private void WriteJSONFile(string fileShortName)
        {
            _levelData.SerializeMeals(_foodCollection);
            _levelData.Validate();
            using var fs = File.Open(FullFilePath(fileShortName), FileMode.Create);
            using var writer = new StreamWriter(fs);
            writer.Write(JsonConvert.SerializeObject(_levelData));
        }

        public static bool GetLevelDataFromJSON(TextAsset file, ref LevelData data, FoodCollection foodCollection)
        {
            if (file == null) return false;

            data = JsonConvert.DeserializeObject<LevelData>(file.text);
            data.DeserializeMeals(foodCollection);
            return true;
        }

        private bool GetLevelDataFromJSON(string path)
        {
            var file = Resources.Load<TextAsset>(path);
            if (file == null)
                return false;
            _levelData = JsonConvert.DeserializeObject<LevelData>(file.text);
            return true;
        }
        
    }
}