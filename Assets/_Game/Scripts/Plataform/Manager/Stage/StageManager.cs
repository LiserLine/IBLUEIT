using Ibit.Core.Audio;
using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Plataform.Logger;
using Ibit.Plataform.Manager.Score;
using Ibit.Plataform.Manager.Spawn;
using NaughtyAttributes;
using UnityEngine;

namespace Ibit.Plataform.Manager.Stage
{
    public partial class StageManager : MonoBehaviour
    {
        #region Events

        public delegate void StageStartHandler();

        public event StageStartHandler OnStageStart;

        public delegate void StageTimeUpHandler();

        public event StageTimeUpHandler OnStageTimeUp;

        public delegate void StageEndHandler();

        public event StageEndHandler OnStageEnd;

        #endregion Events

        #region Properties

        public float Watch { get; private set; }
        public bool IsRunning { get; private set; }
        public float Duration { get; private set; }

        #endregion Properties

        private bool isTimedUp;
        private Spawner spawner;

        [SerializeField]
        private Data.Stage testStage;

        private void Awake()
        {
#if UNITY_EDITOR
            if (Data.Stage.Loaded == null)
                Data.Stage.Loaded = testStage;
            else
                testStage = Data.Stage.Loaded;
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
                    GameManager.ResumeGame();
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

        FindObjectOfType<CanvasManager>().PauseGame();
        SysMessage.Warning("Pitaco foi desconectado.\nReconecte o controle.");
    }
#endif

        private void TimeUp()
        {
            isTimedUp = true;
            SoundManager.Instance.PlaySound("PlataformTimeUp");
            OnStageTimeUp?.Invoke();
        }

        [Button("End Stage")]
        private void EndStage()
        {
            if (!isTimedUp)
            {
                TimeUp();
                return;
            }

            GameOver();
        }

        private void GameOver()
        {
            Watch = 0;
            isTimedUp = true;
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
            Watch += Time.deltaTime;

            if (Watch > Data.Stage.Loaded.SpawnDuration)
                TimeUp();

            if (isTimedUp && spawner.ObjectsOnScene <= 0)
                EndStage();
        }
    }
}