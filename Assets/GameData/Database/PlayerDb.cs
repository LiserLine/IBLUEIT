using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public class PlayerDb
{
    public readonly List<Player> PlayerList;
    public bool IsLoaded { get; private set; }

    public PlayerDb()
    {
        PlayerList = new List<Player>();

        if (File.Exists(GameConstants.PacientListFile))
            Load();
        else
            Save();
    }

    public void Load()
    {
        var csvData = GameUtilities.ReadAllText(GameConstants.PacientListFile);
        var grid = CsvParser2.Parse(csvData);
        for (var i = 1; i < grid.Length; i++)
        {
            if (string.IsNullOrEmpty(grid[i][0])) continue;

            var plr = new Player
            {
                Id = uint.Parse(grid[i][0]),
                Name = grid[i][1],
                Birthday = DateTime.ParseExact(grid[i][2], @"dd/MM/yyyy", CultureInfo.InvariantCulture),
                Observations = grid[i][3],
                Disfunction = (Disfunctions)Enum.Parse(typeof(Disfunctions), grid[i][4]),
                InspiratoryPeakFlow = float.Parse(grid[i][5].Replace('.', ',')),
                ExpiratoryPeakFlow = float.Parse(grid[i][6].Replace('.', ',')),
                InspiratoryFlowTime = float.Parse(grid[i][7].Replace('.', ',')),
                ExpiratoryFlowTime = float.Parse(grid[i][8].Replace('.', ',')),
                RespirationFrequency = float.Parse(grid[i][9].Replace('.', ',')),
                LastLevel = byte.Parse(grid[i][10]),
                OpenLevel = byte.Parse(grid[i][11]),
                TotalScore = uint.Parse(grid[i][12]),
                SessionsDone = uint.Parse(grid[i][13]),
                TutorialDone = bool.Parse(grid[i][14])
            };

            PlayerList.Add(plr);
        }

        IsLoaded = true;
    }

    public void Save()
    {
        var items = new[] { "Id", "Name", "Birthday", "Observations", "Disfunction",
            "InspiratoryPeakFlow", "ExpiratoryPeakFlow", "InspiratoryFlowTime", "ExpiratoryFlowTime", "RespirationFrequency",
            "LastLevel", "OpenLevel", "TotalScore", "SessionsDone", "TutorialDone" };

        var sb = new StringBuilder();
        sb.AppendLine(items.Aggregate((a, b) => a + ";" + b));

        for (var i = 0; i < PlayerList.Count; i++)
        {
            var plr = GetAt(i);
            sb.AppendLine(
                $"{plr.Id};{plr.Name};{plr.Birthday:dd/MM/yyyy};{plr.Observations};{plr.Disfunction};" +
                $"{plr.InspiratoryPeakFlow};{plr.ExpiratoryPeakFlow};{plr.InspiratoryFlowTime};{plr.ExpiratoryFlowTime};" +
                $"{plr.RespirationFrequency};{plr.LastLevel};{plr.OpenLevel};{plr.TotalScore};{plr.SessionsDone};{plr.TutorialDone};");
        }

        //ToDo - mudar para File append quando arquivo já existir
        GameUtilities.WriteAllText(GameConstants.PacientListFile, sb.ToString());
    }

    public void CreatePlayer(Player plr)
    {
        PlayerList.Add(plr);
        Save();
    }

    public Player GetAt(int i)
    {
        return PlayerList.Count <= i ? null : PlayerList[i];
    }

    public Player GetPlayer(uint id)
    {
        return PlayerList.Find(x => x.Id == id);
    }

    public Player GetPlayer(string name)
    {
        return PlayerList.Find(x => x.Name == name);
    }

    public List<Player> ContainsName(string find)
    {
        return PlayerList.FindAll(x => x.Name.Contains(find));
    }
}