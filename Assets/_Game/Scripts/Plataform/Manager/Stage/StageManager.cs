using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Score;
using Ibit.Plataform.Manager.Spawn;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Ibit.Plataform.Manager.Stage
{
    public partial class StageManager : MonoBehaviour
    {
        #region Events

        public event Action OnStageStart;
        public event Action OnStageEnd;

        #endregion Events

        #region Properties

        public bool IsRunning { get; private set; }
        public float Duration { get; private set; }

        #endregion Properties

        private Spawner spawner;        

        private void Awake()
        {

#if UNITY_EDITOR

            if (StageModel.Loaded == null)
                StageModel.Loaded = testStage;
            else
                testStage = StageModel.Loaded;
#endif

            spawner = FindObjectOfType<Spawner>();

            var sc = FindObjectOfType<SerialController>();
            sc.OnSerialConnected += StartStage;

#if !UNITY_EDITOR
        sc.OnSerialDisconnected += PauseOnDisconnect;
#endif

            FindObjectOfType<Player>().OnPlayerDeath += GameOver;

            Time.timeScale = 1f;
        }

        [Button("Start Stage")]
        private void StartStage()
        {
            if (IsRunning)
            {
                if (GameManager.GameIsPaused)
                    GameManager.UnPauseGame();
                else
                    return;
            }

            FindObjectOfType<SerialController>().StartSamplingDelayed();
            IsRunning = true;
            OnStageStart?.Invoke();
        }

#if !UNITY_EDITOR
    private void PauseOnDisconnect()
    {
        if (GameManager.GameIsPaused)
            return;

        FindObjectOfType<Ibit.Plataform.UI.CanvasManager>().PauseGame();
        SysMessage.Warning("Pitaco foi desconectado.\nReconecte o controle.");
    }
#endif

        [Button("End Stage")]
        private void EndStage()
        {
            GameOver();
        }

        private void GameOver()
        {
            IsRunning = false;
            FindObjectOfType<Scorer>().CalculateResult(FindObjectOfType<Player>().HeartPoins < 1);
            FindObjectOfType<SerialController>().StopSampling();
            FindObjectOfType<PitacoLogger>().StopLogging();
            OnStageEnd?.Invoke();
        }

        private void Update()
        {
            if (!IsRunning)
                return;

            Duration += Time.deltaTime;

            if (spawner.ObjectsOnScene < 1)
                EndStage();
        }
    }
}