using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public class PlayerDb
{
    private readonly List<Player> _playerList = new List<Player>();

    public List<Player> PlayerList
    {
        get
        {
            if (!IsLoaded)
                Load();

            return _playerList;
        }
    }

    public bool IsLoaded { get; private set; }

    public PlayerDb()
    {
        if (File.Exists(GameConstants.PacientListFile))
            Load();
        else
            Save();
    }

    public void Load()
    {
        _playerList.Clear();

        var csvData = GameUtilities.ReadAllText(GameConstants.PacientListFile);
        var grid = CsvParser2.Parse(csvData);
        for (var i = 1; i < grid.Length; i++)
        {
            if (string.IsNullOrEmpty(grid[i][0]))
                continue;

            var plr = new Player
            {
                Id = int.Parse(grid[i][0]),
                Name = grid[i][1],
                Birthday = DateTime.ParseExact(grid[i][2], @"dd/MM/yyyy", CultureInfo.InvariantCulture),
                Observations = grid[i][3],
                Disfunction = (Disfunctions)Enum.Parse(typeof(Disfunctions), grid[i][4]),
                RespiratoryInfo = new RespiratoryInfo
                {
                    InspiratoryPeakFlow = GameUtilities.ParseFloat(grid[i][5]),
                    ExpiratoryPeakFlow = GameUtilities.ParseFloat(grid[i][6]),
                    InspiratoryFlowTime = GameUtilities.ParseFloat(grid[i][7]),
                    ExpiratoryFlowTime = GameUtilities.ParseFloat(grid[i][8]),
                    RespirationFrequency = GameUtilities.ParseFloat(grid[i][9]),
                },
                OpenLevel = int.Parse(grid[i][10]),
                TotalScore = int.Parse(grid[i][11]),
                SessionsDone = int.Parse(grid[i][12]),
                CalibrationDone = bool.Parse(grid[i][13])
            };

            _playerList.Add(plr);
        }

        IsLoaded = true;
    }

    public void Save()
    {
        var items = new[] { "Id", "Name", "Birthday", "Observations", "Disfunction",
            "InspiratoryPeakFlow", "ExpiratoryPeakFlow", "InspiratoryFlowTime", "ExpiratoryFlowTime", "RespirationFrequency",
            "OpenLevel", "TotalScore", "SessionsDone", "CalibrationDone" };

        var sb = new StringBuilder();
        sb.AppendLine(items.Aggregate((a, b) => a + ";" + b));

        for (var i = 0; i < _playerList.Count; i++)
        {
            var plr = GetAt(i);
            sb.AppendLine(
                $"{plr.Id};{plr.Name};{plr.Birthday:dd/MM/yyyy};{plr.Observations};{plr.Disfunction};" +
                $"{plr.RespiratoryInfo.InspiratoryPeakFlow};{plr.RespiratoryInfo.ExpiratoryPeakFlow};{plr.RespiratoryInfo.InspiratoryFlowTime};{plr.RespiratoryInfo.ExpiratoryFlowTime};" +
                $"{plr.RespiratoryInfo.RespirationFrequency};{plr.OpenLevel};{plr.TotalScore};{plr.SessionsDone};{plr.CalibrationDone};");
        }

        GameUtilities.WriteAllText(GameConstants.PacientListFile, sb.ToString());
    }

    public void CreatePlayer(Player plr)
    {
        _playerList.Add(plr);
        Save();
    }

    public Player GetAt(int i)
    {
        return _playerList.Count <= i ? null : _playerList[i];
    }

    public Player GetPlayer(uint id)
    {
        return _playerList.Find(x => x.Id == id);
    }

    public Player GetPlayer(string name)
    {
        return _playerList.Find(x => x.Name == name);
    }

    public List<Player> ContainsName(string find)
    {
        return _playerList.FindAll(x => x.Name.Contains(find));
    }
}