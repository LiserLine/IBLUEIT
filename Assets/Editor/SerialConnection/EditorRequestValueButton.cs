using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SerialConnectionManager))]
public class EditorRequestValueButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Request Values"))
        {
            if (SerialConnectionManager.Instance.IsRequestEnabled)
                SerialConnectionManager.Instance.DisabeRequest();
            else
                SerialConnectionManager.Instance.EnableRequest();
        }
    }
}
