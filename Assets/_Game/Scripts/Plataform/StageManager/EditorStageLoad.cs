#if UNITY_EDITOR

using NaughtyAttributes;
using UnityEngine;

public partial class StageManager
{
    [SerializeField]
    private Stage testStage;

    [Button]
    public void EditorStageLoad()
    {
        if (testStage.Id < 1)
        {
            Debug.LogWarning("Test Stage ID must be greater than 0");
            return;
        }

        testStage = StageDb.Load(testStage.Id);

        if (testStage == null)
        {
            Debug.LogWarning("Stage ID not found!");
            return;
        }

        Stage.Loaded = testStage;

        Debug.Log($"Stage {testStage.Id} loaded!");
    }
}

#endif