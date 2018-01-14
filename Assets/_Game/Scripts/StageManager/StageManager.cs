using NaughtyAttributes;
using UnityEngine;

public partial class StageManager : Singleton<StageManager>
{
    public int PlaySessionTime
    {
        get { return playSessionTime; }
        set { playSessionTime = value; }
    }

    public float Timer => timer;

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
        Player.Instance.OnPlayerDeath += EndStage;
        Spawner.Instance.OnRelaxTimeStart += Spawner_OnRelaxTimeStart;
    }

    private void Spawner_OnRelaxTimeStart()
    {
        timer -= 20f;
        timer = Mathf.Clamp(timer, 0f, PlaySessionTime);
    }

    [Button("Start Stage")]
    private void StartStage()
    {
        isRunning = true;
        OnStageStart?.Invoke();
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
