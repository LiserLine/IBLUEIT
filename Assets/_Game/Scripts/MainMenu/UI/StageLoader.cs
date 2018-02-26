using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI
{
    public class StageLoader : MonoBehaviour
    {
        public Stage stage;

        private void OnEnable() => this.GetComponent<Button>().onClick.AddListener(OnStageSelected);

        private void OnStageSelected()
        {
            if (!Pacient.Loaded.CalibrationDone)
            {
                SysMessage.Warning("Calibração não foi feita!");
                return;
            }

#if !UNITY_EDITOR
        if (!FindObjectOfType<SerialController>().IsConnected)
        {
            SysMessage.Warning("Pitaco não está conectado! Conecte antes de jogar!");
            return;
        }
#endif

            Stage.Loaded = stage;
            FindObjectOfType<SceneLoader>().LoadScene(1);
            Debug.Log($"Stage {stage.Id} loaded.");
        }
    }
}