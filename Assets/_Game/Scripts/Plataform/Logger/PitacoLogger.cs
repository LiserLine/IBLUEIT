﻿using System.Linq;
using UnityEngine;

public class PitacoLogger : Logger<PitacoLogger>
{
    protected override void Awake()
    {
        sb.AppendLine("time;value");
        FindObjectOfType<SerialController>().OnSerialMessageReceived += OnSerialMessageReceived;

        if (FindObjectOfType<StageManager>() != null)
            FindObjectOfType<StageManager>().OnStageEnd += StopLogging;
    }

    protected override void Flush()
    {
        var textData = sb.ToString();

        if (textData.Count(s => s == '\n') < 2)
            return;

        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".csv";
        FileReader.WriteAllText(path, textData);
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!isLogging || msg.Length < 1 || GameManager.GameIsPaused)
            return;

        sb.AppendLine($"{Time.time:F};{Parsers.Float(msg):F}");
    }
}