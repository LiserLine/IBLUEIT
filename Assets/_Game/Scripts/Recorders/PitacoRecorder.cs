using System;
using System.Text;
using UnityEngine;

public class PitacoRecorder : Recorder<PitacoRecorder>
{
    private StringBuilder sb;

    private void Awake()
    {
        StageManager.Instance.OnStageStart += StartRecord;
        StageManager.Instance.OnStageEnd += StopRecord;
        SerialController.Instance.OnSerialMessageReceived += OnSerialMessageReceived;

        sb = new StringBuilder();
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!isRecording || msg.Length < 1)
            return;

        sb.AppendLine($"{Time.time:F};{Utils.ParseFloat(msg):F}");
    }

    protected override void StopRecord()
    {
        base.StopRecord();
        FlushData();
    }

    private void FlushData()
    {
        if (sb.Length < 0)
            return;

        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".csv";

        sb.Insert(0, "time;value" + Environment.NewLine);

        Utils.WriteAllText(path, sb.ToString());
    }
}
