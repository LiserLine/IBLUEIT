using UnityEditor;
using UnityEngine;

public class LocalizedTextEditor : EditorWindow
{
    public LocalizationData LocalizationData;

    [MenuItem("Window/Localized Text Editor")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(LocalizedTextEditor)).Show();
    }
    
    private void OnGUI()
    {
        if (LocalizationData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("LocalizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();
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
        string filePath = EditorUtility.OpenFilePanel("LOAD localization data file...", Application.streamingAssetsPath,
            "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = GameUtilities.ReadAllText(filePath);
            LocalizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }

    private void Save()
    {
        string filePath = EditorUtility.SaveFilePanel("SAVE localization data file...", Application.streamingAssetsPath,
            "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(LocalizationData);
            GameUtilities.WriteAllText(filePath, dataAsJson);
        }
    }

    private void CreateNewData()
    {
        LocalizationData = new LocalizationData();
    }
}
