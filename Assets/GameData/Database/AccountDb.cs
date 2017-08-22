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
            Save();
    }

    public void Load()
    {
        var csvData = GameUtilities.ReadAllText(GameConstants.SummaryCsvPath);
        var grid = CsvParser2.Parse(csvData);
        for (var i = 1; i < grid.Length; i++)
        {
            var account = new Account
            {
                Id = uint.Parse(grid[i][0]),
                Name = grid[i][1],
                Birthday = DateTime.ParseExact(grid[i][2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Observations = grid[i][3],
                Disfunction = (Disfunctions)Enum.Parse(typeof(Disfunctions), grid[i][4])
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
                $"{account.Id};{account.Name};{account.Birthday:yyyy-MM-dd};{account.Observations};{account.Disfunction}");
        }

        GameUtilities.WriteAllText(GameConstants.SummaryCsvPath, sb.ToString());
    }

    public void CreateAccount(Account account)
    {
        AccountList.Add(account);
        Save();
    }

    public Account GetAt(int i)
    {
        return AccountList.Count <= i ? null : AccountList[i];
    }

    public Account GetAccount(uint id)
    {
        return AccountList.Find(x => x.Id == id);
    }

    public Account GetAccount(string name)
    {
        return AccountList.Find(x => x.Name == name);
    }

    public List<Account> ContainsName(string find)
    {
        return AccountList.FindAll(x => x.Name.Contains(find));
    }
}