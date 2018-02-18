using UnityEditor;
using UnityEngine;

public class LocalizedTextEditor : EditorWindow
{
    public LocalizationData LocalizationData;

    private Vector2 scrollPos;

    [MenuItem("Window/Localized Text Editor")]
    private static void Init() => GetWindow(typeof(LocalizedTextEditor)).Show();

    private void OnGUI()
    {
        if (LocalizationData != null)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            var serializedObject = new SerializedObject(this);
            var serializedProperty = serializedObject.FindProperty("LocalizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Save data"))
        {
            Save();
        }

        if (GUILayout.Button("Load data"))
        {
            Load();
        }

        if (GUILayout.Button("Create new data"))
        {
            CreateNewData();
        }
    }

    private void Load()
    {
        var filePath = EditorUtility.OpenFilePanel("LOAD localization data file...", Application.streamingAssetsPath, "json");

        if (string.IsNullOrEmpty(filePath))
            return;

        var dataAsJson = FileReader.ReadAllText(filePath);
        LocalizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
    }

    private void Save()
    {
        var filePath = EditorUtility.SaveFilePanel("SAVE localization data file...", Application.streamingAssetsPath, "", "json");

        if (string.IsNullOrEmpty(filePath))
            return;

        var dataAsJson = JsonUtility.ToJson(LocalizationData, true);
        FileReader.WriteAllText(filePath, dataAsJson);
    }

    private void CreateNewData()
    {
        LocalizationData = new LocalizationData
        {
            Items = new[]
            {
                new LocalizationItem
                {
                    Key = "SampleKey",
                    Value = "SampleValue"
                }
            }
        };
    }
}
