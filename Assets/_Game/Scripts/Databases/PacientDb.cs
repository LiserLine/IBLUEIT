using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public class PacientDb
{
    public static PacientDb Instance = new PacientDb();

    private readonly List<Pacient> pacientList = new List<Pacient>();
    private const string filePath = @"savedata/pacients/_pacientList.csv";

    public List<Pacient> PlayerList
    {
        get
        {
            if (!IsLoaded)
                Load();

            return pacientList;
        }
    }

    public bool IsLoaded { get; private set; }

    public PacientDb()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("PacientDb already instanciated.");

        if (File.Exists(filePath))
            Load();
        else
            Save();
    }

    public void Load()
    {
        pacientList.Clear();

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
                    RespiratoryFrequency = Utils.ParseFloat(grid[i][9]),
                },
                StagesOpened = int.Parse(grid[i][10]),
                TotalScore = Utils.ParseFloat(grid[i][11]),
                PlaySessionsDone = int.Parse(grid[i][12]),
                CalibrationDone = bool.Parse(grid[i][13])
            };

            pacientList.Add(plr);
        }
        
        IsLoaded = true;
    }

    public void Save()
    {
        var items = new[] { "Id", "Name", "Birthday", "Observations", "Disfunction",
            "InspiratoryPeakFlow", "ExpiratoryPeakFlow", "InspiratoryFlowTime", "ExpiratoryFlowTime", "RespiratoryFrequency",
            "StagesOpened", "TotalScore", "PlaySessionsDone", "CalibrationDone" };

        var sb = new StringBuilder();
        sb.AppendLine(items.Aggregate((a, b) => a + ";" + b));

        for (var i = 0; i < pacientList.Count; i++)
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
        pacientList.Add(plr);
        Save();
    }

    public Pacient GetAt(int i) => pacientList.Count <= i ? null : pacientList[i];

    public Pacient GetPlayer(uint id) => pacientList.Find(x => x.Id == id);

    public Pacient GetPlayer(string name) => pacientList.Find(x => x.Name == name);

    public List<Pacient> ContainsName(string find) => pacientList.FindAll(x => x.Name.Contains(find));
}