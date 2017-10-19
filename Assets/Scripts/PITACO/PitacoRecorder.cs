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
    private bool _isRecording;

    public PitacoRecorder()
    {
        _incomingDataDictionary = new Dictionary<long, float>();
        _sb = new StringBuilder();
        _stopwatch = new Stopwatch();
    }

    public void StartRecording()
    {
        if (_isRecording)
            throw new Exception("Pitaco Recorder is already recording.");

        _isRecording = true;
        _recordStart = DateTime.Now;
        _stopwatch.Restart();
    }

    public void StopRecording()
    {
        if (!_isRecording)
            throw new Exception("Pitaco Recorder is not recording.");

        _recordFinish = DateTime.Now;
        _stopwatch.Stop();
        _isRecording = false;
    }

    public void RecordValue(float value)
    {
        if (!_isRecording)
            throw new Exception("You must execute StartRecording to add values.");

        _incomingDataDictionary.Add(_stopwatch.ElapsedMilliseconds, value);
    }

    public void WriteData(Player plr, Stage stg, bool clearRecords = false)
    {
        if (_incomingDataDictionary.Count == 0)
            return;

        UnityEngine.Debug.Log($"Writing {_incomingDataDictionary.Count} values from incoming data dictionary...");

        if (plr != null)
        {
            var configString = new[]
            {
                "SessionId", "PlayerId", "PlayerName", "PlayerDisfunction", "SessionStart", "SessionFinish", "StageId"
            };
            _sb.AppendLine(configString.Aggregate((a, b) => a + ";" + b));

            _sb.AppendLine($"{plr.SessionsDone};{plr.Id};{plr.Name};{plr.Disfunction};{_recordStart};{_recordFinish};{stg.Id};");
            _sb.AppendLine();
        }

        foreach (var pair in _incomingDataDictionary)
        {
            _sb.AppendLine($"{pair.Key};{pair.Value}");
        }

        var filePath = plr != null ? GameConstants.GetSessionsPath(plr) + $"PITACO-SESSION_{plr.SessionsDone}.csv" : GameConstants.SaveDataPath + $"PITACO-SESSION_{_recordStart:yyyyMMdd_HHmmss}.csv";

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
