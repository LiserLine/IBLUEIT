using System;
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

    public delegate void StageEndHandler();
    public static event StageEndHandler OnStageEnd;

    public delegate void StageResetHandler();
    public static event StageResetHandler OnStageReset;

    #endregion

    private bool isRunning;
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

    [Button("End Stage")]
    private void EndStage()
    {
        timer = 0;
        isRunning = false;
        OnStageEnd?.Invoke();
    }

    [Button("Reset Stage")]
    private void ResetStage()
    {
        OnStageReset?.Invoke();
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        timer += Time.deltaTime;

        if (timer > playSessionTime)
            EndStage();
    }
}
