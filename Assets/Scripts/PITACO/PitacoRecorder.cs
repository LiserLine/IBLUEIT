using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

public class PitacoRecorder
{
    public static readonly PitacoRecorder Instance = new PitacoRecorder();

    private readonly StringBuilder _sb;
    private readonly Stopwatch _stopwatch;
    private readonly Dictionary<long, float> _incomingDataDictionary;
    private DateTime _recordStart, _recordFinish;

    public PitacoRecorder()
    {
        _incomingDataDictionary = new Dictionary<long, float>();
        _sb = new StringBuilder();
        _stopwatch = new Stopwatch();
    }

    public void StartRecording()
    {
        _recordStart = DateTime.Now;
        _stopwatch.Start();
    }

    public void StopRecording()
    {
        _recordFinish = DateTime.Now;
        _stopwatch.Stop();
    }

    public void AddIncomingData(float value)
    {
        if (!_stopwatch.IsRunning)
            throw new Exception("You must execute StartRecording to add values.");

        _incomingDataDictionary.Add(_stopwatch.ElapsedMilliseconds, value);
    }

    public void WriteData(Player plr, Stage stg, bool clearRecords = false)
    {
        if (_incomingDataDictionary.Count == 0)
            return;

        UnityEngine.Debug.Log($"Writing {_incomingDataDictionary.Count} values from incoming data dictionary...");

        var configString = new[]
        {
            "SessionId", "PlayerId", "PlayerName", "PlayerDisfunction", "SessionStart", "SessionFinish", "StageId"
        };
        _sb.AppendLine(configString.Aggregate((a, b) => a + ";" + b));

        _sb.AppendLine($"{plr.SessionsDone};{plr.Id};{plr.Name};{plr.Disfunction};{_recordStart};{_recordFinish};{stg.Id};");
        _sb.AppendLine();

        foreach (var pair in _incomingDataDictionary)
        {
            _sb.AppendLine($"{pair.Key};{pair.Value}");
        }

        string filePath;

        filePath = plr != null ? GameConstants.GetSessionsPath(plr) + $"PITACO-SESSION_{plr.SessionsDone}.csv" : GameConstants.SaveDataPath + $"PITACO-SESSION_{_recordStart:yyyyMMdd_HHmmss}.csv";

        GameUtilities.WriteAllText(filePath, _sb.ToString());

        if (clearRecords)
        {
            ClearRecords();
        }
    }

    public void ClearRecords()
    {
        _sb.Clear();
        _incomingDataDictionary.Clear();
        _stopwatch.Reset();
    }

    public void LoadData()
    {
        //ToDo
    }
}
