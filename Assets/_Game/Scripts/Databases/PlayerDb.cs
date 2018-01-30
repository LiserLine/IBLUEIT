using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public class PlayerDb
{
    public static PlayerDb Instance = new PlayerDb();

    private readonly List<Pacient> _playerList = new List<Pacient>();
    private const string filePath = @"savedata/pacients/_list.csv";

    public List<Pacient> PlayerList
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
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("PlayerDb already instanciated.");

        if (File.Exists(filePath))
            Load();
        else
            Save();
    }

    public void Load()
    {
        _playerList.Clear();

        var csvData = Utils.ReadAllText(filePath);
        var grid = CsvParser2.Parse(csvData);
        for (var i = 1; i < grid.Length; i++)
        {
            if (string.IsNullOrEmpty(grid[i][0]))
                continue;

            var plr = new Pacient
            {
                Id = int.Parse(grid[i][0]),
                Name = grid[i][1],
                Birthday = DateTime.ParseExact(grid[i][2], @"dd/MM/yyyy", CultureInfo.InvariantCulture),
                Observations = grid[i][3],
                Disfunction = (DisfunctionType)Enum.Parse(typeof(DisfunctionType), grid[i][4]),
                RespiratoryData = new RespiratoryData
                {
                    InspiratoryPeakFlow = Utils.ParseFloat(grid[i][5]),
                    ExpiratoryPeakFlow = Utils.ParseFloat(grid[i][6]),
                    InspiratoryFlowTime = Utils.ParseFloat(grid[i][7]),
                    ExpiratoryFlowTime = Utils.ParseFloat(grid[i][8]),
                    RespirationFrequency = Utils.ParseFloat(grid[i][9]),
                },
                StagesOpened = int.Parse(grid[i][10]),
                TotalScore = Utils.ParseFloat(grid[i][11]),
                PlaySessionsDone = int.Parse(grid[i][12]),
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
            "StagesOpened", "TotalScore", "PlaySessionsDone", "CalibrationDone" };

        var sb = new StringBuilder();
        sb.AppendLine(items.Aggregate((a, b) => a + ";" + b));

        for (var i = 0; i < _playerList.Count; i++)
        {
            var data = GetAt(i);
            var rawInfo = data.RespiratoryData.GetRawInfo();

            sb.AppendLine(
                $"{data.Id};{data.Name};{data.Birthday:dd/MM/yyyy};{data.Observations};{data.Disfunction};" +
                $"{rawInfo.InspiratoryPeakFlow};{rawInfo.ExpiratoryPeakFlow};{rawInfo.InspiratoryFlowTime};{rawInfo.ExpiratoryFlowTime};" +
                $"{rawInfo.RespirationFrequency};{data.StagesOpened};{Math.Truncate(data.TotalScore)};{data.PlaySessionsDone};{data.CalibrationDone};");
        }

        Utils.WriteAllText(filePath, sb.ToString());
    }

    public void CreatePlayer(Pacient plr)
    {
        _playerList.Add(plr);
        Save();
    }

    public Pacient GetAt(int i) => _playerList.Count <= i ? null : _playerList[i];

    public Pacient GetPlayer(uint id) => _playerList.Find(x => x.Id == id);

    public Pacient GetPlayer(string name) => _playerList.Find(x => x.Name == name);

    public List<Pacient> ContainsName(string find) => _playerList.FindAll(x => x.Name.Contains(find));
}