using NaughtyAttributes;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int PlaySessionTime
    {
        get { return playSessionTime; }
        set { playSessionTime = value; }
    }

    #region Events

    public delegate void StageStartHandler();
    public static event StageStartHandler OnStageStart;

    public delegate void StageTimeUpHandler();
    public static event StageTimeUpHandler OnStageTimeUp;

    public delegate void StageEndHandler();
    public static event StageEndHandler OnStageEnd;

    #endregion

    private bool isRunning, isTimedUp;
    private float timer;

    [SerializeField]
    private int playSessionTime = 30;

    private void OnEnable()
    {
        SerialController.OnSerialConnected += StartStage;
        Player.OnPlayerDeath += EndStage;
        Spawner.OnRelaxTimeStart += Spawner_OnRelaxTimeStart;
    }

    private void Spawner_OnRelaxTimeStart()
    {
        timer -= 20f;
    }

    private void OnDisable()
    {
        SerialController.OnSerialConnected -= StartStage;
        Player.OnPlayerDeath -= EndStage;
        Spawner.OnRelaxTimeStart -= Spawner_OnRelaxTimeStart;
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
        if(!isTimedUp)
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

        if (isTimedUp && Spawner.ObjectsRemaining == 0)
            EndStage();
    }
}
