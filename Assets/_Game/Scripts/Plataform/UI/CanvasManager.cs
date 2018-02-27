using Ibit.Core.Database;
using Ibit.Core.Game;
using Ibit.Plataform.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.Plataform.UI
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField]
        private Text stageLevel;

        [SerializeField]
        private Text stagePhase;

        [SerializeField]
        private GameObject pauseMenu;

        private void OnEnable()
        {
            stageLevel.text = Stage.Loaded.Level.ToString();
            stagePhase.text = ((int)Stage.Loaded.ObjectToSpawn).ToString();
        }

        public void PauseGame()
        {
            pauseMenu.SetActive(true);
            GameManager.PauseGame();
        }

        public void UnPauseGame()
        {
            pauseMenu.SetActive(false);
            GameManager.ResumeGame();
        }

        public void SetNextStage()
        {
#if UNITY_EDITOR
            Stage.Loaded = StageDb.Instance == null
                ? StageDb.Load(Stage.Loaded.Id + 1)
                : StageDb.Instance.GetStage(Stage.Loaded.Id + 1);
#else
        Stage.Loaded = StageDb.Instance.GetStage(Stage.Loaded.Id + 1 > StageDb.Instance.StageList.Max(x => x.Id) ? 1 : Stage.Loaded.Id + 1);
#endif
        }
    }
}