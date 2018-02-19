using System.IO;
using UnityEngine;

public class PlataformLogger : Logger
{
    private Player plr;
    private Scorer scr;
    private Spawner spwn;

    protected override void Awake()
    {
        base.Awake();
        sb.AppendLine("time;tag;instanceId;posX;posY");
        plr = FindObjectOfType<Player>();
        spwn = FindObjectOfType<Spawner>();
        scr = FindObjectOfType<Scorer>();
        FindObjectOfType<StageManager>().OnStageEnd += StopLogging;
    }

    protected override void Flush()
    {
        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".csv";
        FileReader.WriteAllText(path, sb.ToString());
        LogPlaySession();
    }

    private void LogPlaySession()
    {
        if (scr.Result == GameResult.Success)
        {
            if (Stage.Loaded.Id == Pacient.Loaded.UnlockedLevels)
                Pacient.Loaded.UnlockedLevels++;
        }
        else
        {
            if (scr.Score < scr.MaxScore * 0.3f)
                Pacient.Loaded.UnlockedLevels--;

            if (Pacient.Loaded.UnlockedLevels == 0)
                Pacient.Loaded.UnlockedLevels = 1;
        }

        Pacient.Loaded.PlaySessionsDone++;
        Pacient.Loaded.AccumulatedScore += scr.Score;
        PacientDb.Instance?.Save();

        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/_PlataformHistory.csv";

        var data = $"{recordStart};{recordStop};{scr.Result};" +
                   $"{Stage.Loaded.Id};{Stage.Loaded.SpawnObject};{Stage.Loaded.Level};{spwn.RelaxTimeSpawned};" +
                   $"{scr.Score};{scr.MaxScore};{scr.Score / scr.MaxScore};" +
                   $"{spwn.TargetsSucceeded + spwn.TargetsFailed};{spwn.TargetsSucceeded};{spwn.TargetsFailed};" +
                   $"{spwn.ObstaclesSucceeded + spwn.ObstaclesFailed};{spwn.ObstaclesSucceeded};{spwn.ObstaclesFailed};" +
                   $"{plr.HeartPoins};";

        sb.Clear();

        if (!File.Exists(path))
        {
            sb.AppendLine($"Pacient: {Pacient.Loaded.Id}");
            sb.AppendLine($"Name: {Pacient.Loaded.Name}");
            sb.AppendLine($"Condition: {Pacient.Loaded.Condition}");
            sb.AppendLine();
            sb.AppendLine("playStart;playFinish;result;" +
                          "stageId;phase;level;relaxTimeSpawned;" +
                          "score;maxScore;scoreRatio;" +
                          "targetsSpawned;targetsSuccess;targetsFail;" +
                          "obstaclesSpawned;obstaclesSuccess;obstaclesFail;" +
                          "playerHP;");

            sb.AppendLine(data);
            FileReader.WriteAllText(path, sb.ToString());
        }
        else
        {
            sb.AppendLine(data);
            FileReader.AppendAllText(path, sb.ToString());
        }

        GameObject.Find("Canvas").transform.Find("ResultScreen").gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isRecording)
            return;

        sb.AppendLine($"{Time.time};{plr.tag};{plr.GetInstanceID()};{plr.transform.position.x:F};{plr.transform.position.y:F}");

        foreach (var o in spwn.SpawnedObjects)
        {
            if (o != null)
                sb.AppendLine($"{Time.time};{o.tag};{o.GetInstanceID()};{o.position.x:F};{o.position.y:F}");
        }
    }
}