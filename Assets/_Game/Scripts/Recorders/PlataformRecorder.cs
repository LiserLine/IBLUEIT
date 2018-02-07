using System.IO;
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

        foreach (var o in Spawner.Instance.SpawnedObjects)
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
        FlushPlaySession();
        sb.Clear();
        UpdatePlataformHistory();
    }

    private void FlushPlaySession()
    {
        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".csv";
        sb.Insert(0, $"{PlataformCsvKeys}\n{GetPlataformData()}\n\n{ObjectsCsvKeys}\n");
        Utils.WriteAllText(path, sb.ToString());
    }

    private const string ObjectsCsvKeys = "time;tag;instanceId;posX;posY";

    private const string PlataformCsvKeys = "StageId;Start;Stop;InsHeightLevel;ExpHeightLevel;ExpSizeLevel;" +
                                            "Score;MaxScore;Resistance;PitacoThreshold;PlataformMinScoreMultiplier";

    private string GetPlataformData() =>
        $"{Spawner.StageToLoad};{recordStart:s};{recordStop:s};{Spawner.Instance.InspiratoryHeightLevel};" +
        $"{Spawner.Instance.ExpiratoryHeightLevel};{Spawner.Instance.ExpiratorySizeLevel};{Scorer.Instance.Score};" +
        $"{Scorer.Instance.MaxScore};{GameMaster.Resistance};{GameMaster.PitacoThreshold};{GameMaster.PlataformMinScoreMultiplier}";

    private void UpdatePlataformHistory()
    {
        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/_PlataformHistory.csv";

        if (!File.Exists(path))
        {
            sb.AppendLine(PlataformCsvKeys);
            sb.Append(GetPlataformData());
            Utils.WriteAllText(path, sb.ToString());
        }
        else
            File.AppendAllText(path, $"\n{GetPlataformData()}");
    }
}
