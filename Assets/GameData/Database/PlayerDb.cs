using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public class PlayerDb
{
    public readonly List<Player> PlayerList;
    public bool IsLoaded { get; private set; }

    public PlayerDb()
    {
        PlayerList = new List<Player>();

        if (File.Exists(GameConstants.PacientListPath))
            Load();
        else
            Save();
    }

    public void Load()
    {
        var csvData = GameUtilities.ReadAllText(GameConstants.PacientListPath);
        var grid = CsvParser2.Parse(csvData);
        for (var i = 1; i < grid.Length; i++)
        {
            var plr = new Player
            {
                Id = uint.Parse(grid[i][0]),
                Name = grid[i][1],
                Birthday = DateTime.ParseExact(grid[i][2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Observations = grid[i][3],
                Disfunction = (Disfunctions)Enum.Parse(typeof(Disfunctions), grid[i][4])
            };

            PlayerList.Add(plr);
        }

        IsLoaded = true;
    }

    public void Save()
    {
        var firstLine = "Id;Name;Birthday;Observations;Disfunction;" +
                        "InspiratoryPeakFlow;ExpiratoryPeakFlow;InspiratoryFlowTime;ExpiratoryFlowTime;" +
                        "RespirationFrequency;LastPhase;OpenPhases;TotalScore;SessionsDone;TutorialDone;";

        var sb = new StringBuilder();
        sb.AppendLine(firstLine);

        for (var i = 0; i < PlayerList.Count; i++)
        {
            var plr = GetAt(i);
            sb.AppendLine(
                $"{plr.Id};{plr.Name};{plr.Birthday:yyyy-MM-dd};{plr.Observations};{plr.Disfunction};" + 
                $"{plr.InspiratoryPeakFlow};{plr.ExpiratoryPeakFlow};{plr.InspiratoryFlowTime};{plr.ExpiratoryFlowTime};" +
                $"{plr.RespirationFrequency};{plr.LastPhase};{plr.OpenPhases};{plr.TotalScore};{plr.SessionsDone};{plr.TutorialDone};");
        }

        GameUtilities.WriteAllText(GameConstants.PacientListPath, sb.ToString());
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