using NaughtyAttributes;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public int PlaySessionTime
    {
        get { return playSessionTime; }
        set { playSessionTime = value; }
    }

    public float Timer => timer;
    public bool IsRunning => isRunning;

    public delegate void StageStartHandler();
    public event StageStartHandler OnStageStart;

    public delegate void StageTimeUpHandler();
    public event StageTimeUpHandler OnStageTimeUp;

    public delegate void StageEndHandler();
    public event StageEndHandler OnStageEnd;

    private bool isRunning, isTimedUp;
    private float timer;

    [SerializeField]
    private int playSessionTime = 30;

    private void OnEnable()
    {
        SerialController.Instance.OnSerialConnected += StartStage;
        SerialController.Instance.OnSerialDisconnected += PauseOnDisconnect;
        Player.Instance.OnPlayerDeath += EndStage;
        Time.timeScale = 1f;
    }

    [Button("Start Stage")]
    private void StartStage()
    {
        if (isRunning)
        {
            if (GameMaster.Instance.GameIsPaused)
                GameMaster.Instance.UnPauseGame();
            else
                return;
        }
        
        isRunning = true;
        OnStageStart?.Invoke();
    }

    private void PauseOnDisconnect()
    {
        SysMessage.Warning("Pitaco foi desconectado.\nReconecte o controle.");
        GameMaster.Instance.PauseGame();
    }

    private void TimeUp()
    {
        isTimedUp = true;
        OnStageTimeUp?.Invoke();
    }

    [Button("End Stage")]
    private void EndStage()
    {
        if (!isTimedUp)
            TimeUp();

        timer = 0;
        isRunning = false;
        OnStageEnd?.Invoke();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        timer += Time.deltaTime;

        if (timer > playSessionTime)
            TimeUp();
        
        if (isTimedUp && Spawner.Instance.ObjectsOnScene == 0)
            EndStage();
    }
}
