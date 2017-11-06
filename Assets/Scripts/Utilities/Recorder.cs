using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class Recorder
{
    protected readonly Stopwatch Stopwatch;
    protected DateTime RecordStart, RecordFinish;
    protected bool IsRecording;
    protected readonly string RecorderName;

    protected Recorder(string recorderName)
    {
        Stopwatch = new Stopwatch();
        RecorderName = recorderName;
    }

    public virtual void StartRecording()
    {
        if (IsRecording)
            throw new Exception($"Recorder {RecorderName} is already recording.");

        IsRecording = true;
        RecordStart = DateTime.Now;

        UnityEngine.Debug.Log($"Recorder {RecorderName} started recording at {RecordStart}");
        Stopwatch.Restart();
    }

    public virtual void StopRecording()
    {
        if (!IsRecording)
            throw new Exception($"Recorder {RecorderName} is not recording.");

        Stopwatch.Stop();
        RecordFinish = DateTime.Now;
        IsRecording = false;

        UnityEngine.Debug.Log($"Recorder {RecorderName} finished recording - {RecordFinish}");
    }

    public virtual void ClearRecords()
    {
        Stopwatch.Reset();
    }

    public abstract void WriteData(Player plr, Stage stg, bool clearRecords = false);

    public abstract void LoadData(string filepath);
}

public class GameSessionRecorder : Recorder
{
    private readonly StringBuilder _sb;
    private int _numLines;

    public GameSessionRecorder(string recorderName) : base(recorderName)
    {
        _sb = new StringBuilder();
    }

    public override void LoadData(string filepath)
    {
        throw new NotImplementedException();
    }

    public override void WriteData(Player plr, Stage stg, bool clearRecords = false)
    {
        if (_numLines == 0)
            return;

        var filePath = GameConstants.GetSessionsPath(plr) + $"{plr.SessionsDone}_GAME-SESSION.csv";

        if (File.Exists(filePath))
            return;

        UnityEngine.Debug.Log($"({RecorderName}) Writing {_numLines} values from game session...");

        _sb.Insert(0, "time;tag;id;positionX;positionY\n");

        GameUtilities.WriteAllText(filePath, _sb.ToString());
    }

    public void RecordValue(float time, string tag, int id, Vector2 position)
    {
        _sb.AppendLine($"{time:F};{tag};{id};{position.x:F};{position.y:F}");
        _numLines++;
    }
}

public class PitacoRecorder : Recorder
{
    private readonly StringBuilder _sb;
    private readonly Dictionary<long, float> _incomingDataDictionary;

    public PitacoRecorder(string recorderName) : base(recorderName)
    {
        _incomingDataDictionary = new Dictionary<long, float>();
        _sb = new StringBuilder();
    }

    public void RecordValue(float value)
    {
        if (!IsRecording)
            throw new Exception($"({RecorderName}) You must execute StartRecording to add values.");

        _incomingDataDictionary.Add(Stopwatch.ElapsedMilliseconds, value);
    }

    public override void WriteData(Player plr, Stage stg, bool clearRecords = false)
    {
        if (_incomingDataDictionary.Count == 0)
            return;

        var filePath = plr != null ? GameConstants.GetSessionsPath(plr) + $"{plr.SessionsDone}_PITACO-SESSION.csv" : GameConstants.SaveDataPath + $"{RecordStart:yyyyMMdd_HHmmss}_PITACO-SESSION.csv";

        if (File.Exists(filePath))
            return;

        UnityEngine.Debug.Log($"({RecorderName}) Writing {_incomingDataDictionary.Count} values from incoming data dictionary...");

        _sb.Insert(0, "time;value\n");

        foreach (var pair in _incomingDataDictionary)
            _sb.AppendLine($"{pair.Key};{pair.Value}");

        GameUtilities.WriteAllText(filePath, _sb.ToString());

        if (clearRecords)
            ClearRecords();
    }

    public override void ClearRecords()
    {
        _sb.Clear();
        _incomingDataDictionary.Clear();

        base.ClearRecords();
    }

    public override void LoadData(string filepath)
    {
        throw new NotImplementedException();
    }
}
