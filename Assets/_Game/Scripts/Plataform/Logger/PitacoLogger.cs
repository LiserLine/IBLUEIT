using UnityEngine;

public class PitacoLogger : Logger<PitacoLogger>
{
    protected override void Awake()
    {
        base.Awake();
        sb.AppendLine("time;value");
        FindObjectOfType<SerialController>().OnSerialMessageReceived += OnSerialMessageReceived;

        if (FindObjectOfType<StageManager>() != null)
            FindObjectOfType<StageManager>().OnStageEnd += StopLogging;
    }

    protected override void Flush()
    {
        if (sb.Length < 0)
            return;

        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".csv";
        FileReader.WriteAllText(path, sb.ToString());
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!isLogging || msg.Length < 1)
            return;

        sb.AppendLine($"{Time.time:F};{Parsers.Float(msg):F}");
    }
}