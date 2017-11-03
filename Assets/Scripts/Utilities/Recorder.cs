using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    private readonly Dictionary<long, float> _positionDictionary;

    public GameSessionRecorder(string recorderName) : base(recorderName)
    {
        _positionDictionary = new Dictionary<long, float>();
        _sb = new StringBuilder();
    }

    public override void LoadData(string filepath)
    {
        throw new NotImplementedException();
    }

    private Vector2 ParseVector2(string str)
    {
        throw new NotImplementedException();
    }

    public override void WriteData(Player plr, Stage stg, bool clearRecords = false)
    {
        if (_positionDictionary.Count == 0)
            return;

        UnityEngine.Debug.Log($"({RecorderName}) Writing {_positionDictionary.Count} values from position dictionary...");

        if (plr != null)
        {
            var configString = new[]
            {
                "SessionId", "PlayerId", "PlayerName", "PlayerDisfunction", "SessionStart", "SessionFinish", "StageId"
            };
            _sb.AppendLine(configString.Aggregate((a, b) => a + ";" + b));

            _sb.AppendLine($"{plr.SessionsDone};{plr.Id};{plr.Name};{plr.Disfunction};{RecordStart};{RecordFinish};{stg.Id};");
            _sb.AppendLine();
        }

        foreach (var pair in _positionDictionary)
        {
            _sb.AppendLine($"{pair.Key};{pair.Value:F}");
        }

        var filePath = plr != null ? GameConstants.GetSessionsPath(plr) + $"GAME-SESSION_{plr.SessionsDone}.csv" : GameConstants.SaveDataPath + $"GAME-SESSION_{RecordStart:yyyyMMdd_HHmmss}.csv";

        GameUtilities.WriteAllText(filePath, _sb.ToString());

        if (clearRecords)
        {
            ClearRecords();
        }
    }

    public void RecordValue(float Yposition)
    {
        _positionDictionary.Add(Stopwatch.ElapsedMilliseconds, Yposition);
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

        UnityEngine.Debug.Log($"({RecorderName}) Writing {_incomingDataDictionary.Count} values from incoming data dictionary...");

        if (plr != null)
        {
            var configString = new[]
            {
                "SessionId", "PlayerId", "PlayerName", "PlayerDisfunction", "SessionStart", "SessionFinish", "StageId"
            };
            _sb.AppendLine(configString.Aggregate((a, b) => a + ";" + b));

            _sb.AppendLine($"{plr.SessionsDone};{plr.Id};{plr.Name};{plr.Disfunction};{RecordStart};{RecordFinish};{stg.Id};");
            _sb.AppendLine();
        }

        foreach (var pair in _incomingDataDictionary)
        {
            _sb.AppendLine($"{pair.Key};{pair.Value}");
        }

        var filePath = plr != null ? GameConstants.GetSessionsPath(plr) + $"PITACO-SESSION_{plr.SessionsDone}.csv" : GameConstants.SaveDataPath + $"PITACO-SESSION_{RecordStart:yyyyMMdd_HHmmss}.csv";

        GameUtilities.WriteAllText(filePath, _sb.ToString());

        if (clearRecords)
        {
            ClearRecords();
        }
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
