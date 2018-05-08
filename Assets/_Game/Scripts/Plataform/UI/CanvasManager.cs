using Ibit.Core.Database;
using Ibit.Core.Game;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Score;
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

        [SerializeField]
        private GameObject helpPanel;

        [SerializeField]
        private GameObject _resultPanel;

        private void OnEnable()
        {
            stageLevel.text = StageModel.Loaded.Level.ToString();
            stagePhase.text = StageModel.Loaded.Phase.ToString();
        }

        public void PauseGame()
        {
            if (_resultPanel.activeSelf)
                return;

            if (GameManager.GameIsPaused)
                return;

            helpPanel.SetActive(true);
            pauseMenu.SetActive(true);
            GameManager.PauseGame();
        }

        public void UnPauseGame()
        {
            if (_resultPanel.activeSelf)
                return;

            if (!GameManager.GameIsPaused)
                return;

            helpPanel.SetActive(false);
            pauseMenu.SetActive(false);
            GameManager.UnPauseGame();
        }

        public void SetNextStage()
        {
            StageModel.Loaded = StageDb.Instance.GetStage(
                StageModel.Loaded.Id + 1 > StageDb.Instance.StageList.Max(x => x.Id) ? 1 : StageModel.Loaded.Id + 1);
        }
    }
}