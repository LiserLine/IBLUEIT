using System.Text;
using UnityEngine;

public class PlataformRecorder : Recorder<PlataformRecorder>
{
    private StringBuilder sb;

    private void Awake()
    {
        StageManager.Instance.OnStageStart += StartRecord;
        StageManager.Instance.OnStageEnd += StopRecord;
        sb = new StringBuilder();
    }

    private void Update()
    {
        if (!isRecording)
            return;

        sb.AppendLine($"{Time.time:F};{Player.Instance.tag};{Player.Instance.GetInstanceID()};{Player.Instance.transform.position.x:F};{Player.Instance.transform.position.y:F}");

        foreach (var o in Spawner.Instance.ObjectsOnScene)
        {
            if (o != null)
                sb.AppendLine($"{Time.time:F};{o.tag};{o.GetInstanceID()};{o.transform.position.x:F};{o.transform.position.y:F}");
        }
    }

    protected override void StopRecord()
    {
        base.StopRecord();
        FlushData();
    }

    private void FlushData()
    {
        var path = @"savedata/pacients/" + Player.Data.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".csv";

        sb.Insert(0, "StageId;Start;Stop;InsHeightLevel;ExpHeightLevel;ExpSizeLevel;Score;MaxScore;Mercy;PitacoThreshold;PlataformMinScoreMultiplier\n" +
                     $"{Spawner.StageToLoad};{recordStart:s};{recordStop:s};{Spawner.Instance.InspiratoryHeightLevel};" +
                     $"{Spawner.Instance.ExpiratoryHeightLevel};{Spawner.Instance.ExpiratorySizeLevel};{Scorer.Instance.Score};" +
                     $"{Scorer.Instance.MaxScore};{GameManager.Mercy};{GameManager.PitacoThreshold};{GameManager.PlataformMinScoreMultiplier}" +
                     "\ntime;tag;instanceId;posX;posY\n");

        Utils.WriteAllText(path, sb.ToString());
    }
}
