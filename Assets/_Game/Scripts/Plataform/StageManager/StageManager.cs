using NaughtyAttributes;
using UnityEngine;

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

    private void Awake()
    {
#if UNITY_EDITOR
        if (Stage.Loaded == null)
            Stage.Loaded = testStage;
        else
            testStage = Stage.Loaded;
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

        FindObjectOfType<CanvasManager_P>().PauseGame();
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
        OnStageEnd?.Invoke();
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        Duration += Time.deltaTime;
        Watch += Time.deltaTime;

        if (Watch > Stage.Loaded.SpawnDuration)
            TimeUp();

        if (isTimedUp && spawner.ObjectsOnScene <= 0)
            EndStage();
    }
}