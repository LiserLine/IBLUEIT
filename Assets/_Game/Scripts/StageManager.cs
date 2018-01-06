using System;
using NaughtyAttributes;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public delegate void StageStartHandler();
    public static event StageStartHandler OnStageStart;

    public delegate void StageEndHandler();
    public static event StageEndHandler OnStageEnd;

    public delegate void StageResetHandler();
    public static event StageResetHandler OnStageReset;

    private bool isRunning;
    private float timer;

    [Slider(30, 120)]
    public int playSessionTime = 30;

    public void OnEnable()
    {
        SerialController.OnSerialConnected += StartStage;
        Player.OnPlayerDeath += EndStage;
    }

    public void OnDisable()
    {
        SerialController.OnSerialConnected -= StartStage;
        Player.OnPlayerDeath -= EndStage;
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
