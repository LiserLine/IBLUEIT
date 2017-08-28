using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public void Start()
    {
        UnityEngine.Debug.Log($"Pitaco is now recording...");
        _recordStart = DateTime.Now;
        _stopwatch.Start();
    }

    public void Stop()
    {
        _recordFinish = DateTime.Now;
        _stopwatch.Stop();
    }

    public void Add(float value)
    {
        _incomingDataDictionary.Add(_stopwatch.ElapsedMilliseconds, value);
    }

    public void WriteData(string path = null, bool clear = false)
    {
        UnityEngine.Debug.Log($"Writing {_incomingDataDictionary.Count} values from incoming data dictionary...");

        foreach (var pair in _incomingDataDictionary)
        {
            _sb.AppendLine($"{pair.Key};{pair.Value}");
        }

        string filepath;

        if (string.IsNullOrEmpty(path))
        {
            filepath = GameConstants.SaveDataPath + $"test_{_recordStart:yyyyMMdd_hhmmss}.csv";
        }
        else
        {
            filepath = path + _recordStart.ToString("yyyyMMdd_hhmmss") + ".csv";
        }

        GameUtilities.WriteAllText(filepath, _sb.ToString());

        if (clear)
        {
            Clear();
        }
    }

    public void WriteData(Player plr, Stage stg, string path, bool clear = false)
    {
        _sb.AppendLine("PlayerId;PlayerName;PlayerDisfunction;SessionStart;SessionFinish;StageId;StageSensitivityUsed");
        _sb.AppendLine($"{plr.Id};{plr.Name};{plr.Disfunction};{_recordStart};{_recordFinish};{stg.Id};{stg.SensitivityUsed}");
        _sb.AppendLine();

        WriteData(path, clear);
    }

    public void Clear()
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
