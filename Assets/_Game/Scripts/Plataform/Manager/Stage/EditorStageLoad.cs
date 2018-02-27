#if UNITY_EDITOR

using Ibit.Core.Database;
using NaughtyAttributes;
using UnityEngine;

namespace Ibit.Plataform.Manager.Stage
{
    public partial class StageManager
    {
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

            Data.Stage.Loaded = testStage;

            Debug.Log($"Stage {testStage.Id} loaded!");
        }
    }
}

#endif