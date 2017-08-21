using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public class AccountDb : IDatabase
{
    public readonly List<Account> AccountList;
    public bool IsLoaded { get; private set; }

    public AccountDb()
    {
        AccountList = new List<Account>();

        if (File.Exists(GameConstants.SummaryCsvPath))
            Load();
        else
        {
            Save();
            Load();
        }
    }

    public void Load()
    {
        var csvText = GameConstants.SummaryCsvPath;
        AccountList.Clear();
        var grid = CsvParser2.Parse(csvText);
        for (var i = 1; i < grid.Length; i++)
        {
            var account = new Account
            {
                Id = uint.Parse(grid[i][0]),
                Name = grid[i][1],
                Birthday = DateTime.ParseExact(grid[i][2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Observations = grid[i][3],
                Disfunction = (Disfunctions)Enum.Parse(typeof(Disfunctions), grid[i][4].ToLower())
            };

            AccountList.Add(account);
        }

        IsLoaded = true;
    }

    public void Save()
    {
        var firstLine = "Id;Name;Birthday;Observations;Disfunction";

        var sb = new StringBuilder();
        sb.AppendLine(firstLine);

        for (var i = 0; i < AccountList.Count; i++)
        {
            var account = GetAt(i);
            sb.AppendLine(
                $"{account.Id};{account.Name};{account.Birthday};{account.Observations};{account.Disfunction}");
        }

        GameUtilities.WriteAllText(GameConstants.SummaryCsvPath, sb.ToString());
    }

    public Account GetAt(int i)
    {
        return AccountList.Count <= i ? null : AccountList[i];
    }

    public Account Find_Id(uint find)
    {
        return AccountList.Find(x => x.Id == find);
    }
    public List<Account> FindAll_Id(uint find)
    {
        return AccountList.FindAll(x => x.Id == find);
    }
    public Account Find_Name(string find)
    {
        return AccountList.Find(x => x.Name == find);
    }
    public List<Account> FindAll_Name(string find)
    {
        return AccountList.FindAll(x => x.Name == find);
    }
    public Account Find_Birthday(DateTime find)
    {
        return AccountList.Find(x => x.Birthday == find);
    }
    public List<Account> FindAll_Birthday(DateTime find)
    {
        return AccountList.FindAll(x => x.Birthday == find);
    }
    public Account Find_Observations(string find)
    {
        return AccountList.Find(x => x.Observations == find);
    }
    public List<Account> FindAll_Observations(string find)
    {
        return AccountList.FindAll(x => x.Observations == find);
    }
    public Account Find_Disfunction(Disfunctions find)
    {
        return AccountList.Find(x => x.Disfunction == find);
    }
    public List<Account> FindAll_Disfunction(Disfunctions find)
    {
        return AccountList.FindAll(x => x.Disfunction == find);
    }
}