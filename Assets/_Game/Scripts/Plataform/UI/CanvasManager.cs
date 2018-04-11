using Ibit.Core.Database;
using Ibit.Core.Game;
using Ibit.Plataform.Data;
using System.Linq;
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
            stageLevel.text = StageInfo.Loaded.Level.ToString();
            stagePhase.text = StageInfo.Loaded.Phase.ToString();
        }

        public void PauseGame()
        {
            if (GameManager.GameIsPaused)
                return;

            pauseMenu.SetActive(true);
            GameManager.PauseGame();
        }

        public void UnPauseGame()
        {
            if (!GameManager.GameIsPaused)
                return;

            pauseMenu.SetActive(false);
            GameManager.ResumeGame();
        }

        public void SetNextStage()
        {
            StageInfo.Loaded = StageDb.Instance.GetStage(StageInfo.Loaded.Id + 1 > StageDb.Instance.StageList.Max(x => x.Id) ? 1 : StageInfo.Loaded.Id + 1);

            //#if UNITY_EDITOR
            //            StageInfo.Loaded = StageDb.Instance == null
            //                ? StageDb.Load(StageInfo.Loaded.Id + 1)
            //                : StageDb.Instance.GetStage(StageInfo.Loaded.Id + 1);
            //#else
            //            StageInfo.Loaded = StageDb.Instance.GetStage(StageInfo.Loaded.Id + 1 > StageDb.Instance.StageList.Max(x => x.Id) ? 1 : StageInfo.Loaded.Id + 1);
            //#endif
        }
    }
}