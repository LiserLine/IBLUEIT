using UnityEngine;

public class PlataformSceneManager : MonoBehaviour
{
    private float _dt;

    private void Update()
    {
        if (GameManager.Instance.Stage == null || !GameManager.Instance.Stage.IsRunning || !SerialGetOffset.IsUsingOffset)
            return;

        _dt += Time.deltaTime;

#if false
        if (_dt <= 60)
            return;

        StopStageOnTimeLimit();
#else
        if (_dt <= ((PlataformStage)GameManager.Instance.Stage).TimeLimit)
            return;

        StopStageOnTimeLimit();
#endif

        _dt = 0;
    }

    public void StopStage()
    {
        GameManager.Instance.Stage.Stop();
        SerialGetOffset.ResetData();
        var sComm = GameObject.FindGameObjectWithTag("SerialController");
        Destroy(sComm);
    }

    public void StopStageOnTimeLimit()
    {
        GameManager.Instance.Player.SessionsDone++;
        StopStage();
        CalculateAndWriteStatistics();
    }

    private void CalculateAndWriteStatistics()
    {
        var stage = (PlataformStage)GameManager.Instance.Stage;

        var proportion = stage.Score / stage.SpawnedScore;

        GameManager.Instance.Player.TotalScore += stage.Score;

        if (proportion > GameConstants.Plataform.NextStageOpenProportion && GameManager.Instance.Player.StagesOpened == stage.Id)
        {
            GameManager.Instance.Player.StagesOpened++;
            Debug.Log($"Player opened next stage (ID: {GameManager.Instance.Player.StagesOpened})!");
        }
        else
        {
            Debug.Log($"Not enough score to open next stage! {stage.Score} < {stage.SpawnedScore}");
        }

        Debug.Log($"{stage.Id};{stage.StartTime};{stage.EndTime};{stage.Score};{stage.SpawnedScore};{stage.Elements};" +
                  $"{stage.GameLevel};{stage.HeightLevel};{stage.SizeLevel};{stage.IntervalLevel};{stage.TimeLimit}");

        DatabaseManager.Instance.Players.Save();
    }
}
