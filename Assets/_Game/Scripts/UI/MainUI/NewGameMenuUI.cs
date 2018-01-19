using System;
using UnityEngine;
using UnityEngine.UI;

public class NewGameMenuUI : BasicUI<NewGameMenuUI>
{
    public void CreateNewGame()
    {
        var bDay = GameObject.Find("LabelBDay").GetComponent<Text>().text;
        var bMonth = GameObject.Find("LabelBMonth").GetComponent<Text>().text;
        var bYear = GameObject.Find("LabelBYear").GetComponent<Text>().text;

        DateTime birthday;
        try
        {
            birthday = new DateTime(int.Parse(bYear), int.Parse(bMonth), int.Parse(bDay));
        }
        catch (ArgumentOutOfRangeException)
        {
            SysMessage.Warning("Data invalida!");
            return;
        }

        var playerName = GameObject.Find("InputFieldName").GetComponent<InputField>().text;

        if (playerName.Length == 0)
        {
            SysMessage.Warning("Nome de jogador indefinido!");
            return;
        }

        var normal = GameObject.Find("ToggleNormal").GetComponent<Toggle>().isOn;
        var obstructive = GameObject.Find("ToggleObstructive").GetComponent<Toggle>().isOn;
        var restrictive = GameObject.Find("ToggleRestrictive").GetComponent<Toggle>().isOn;

        if (normal == obstructive == restrictive == false)
        {
            SysMessage.Warning("Disfunção Indefinida!");
            return;
        }

        var disfunction = restrictive ? DisfunctionType.Restrictive
            : (obstructive ? DisfunctionType.Obstructive : DisfunctionType.Normal);

        var observations = GameObject.Find("Observations").GetComponent<InputField>().text;

        var plr = new PlayerData
        {
            Name = playerName,
            Birthday = birthday,
            Disfunction = disfunction,
            Id = PlayerDb.Instance.PlayerList.Count + 1,
            Observations = observations,
            RespiratoryInfo = new RespiratoryInfo(),
            CalibrationDone = false,
            StagesOpened = 1
        };

        var tmpPlr = PlayerDb.Instance.GetPlayer(playerName);

        if (plr.Name.Equals(tmpPlr?.Name)
            && plr.Birthday.Equals(tmpPlr?.Birthday)
            && plr.Disfunction.Equals(tmpPlr?.Disfunction))
        {
            SysMessage.Warning("Jogador existente!");
            return;
        }

        PlayerDb.Instance.CreatePlayer(plr);
        PlayerData.Player = plr;

        SysMessage.Info("Jogador criado com sucesso!");

        this.Hide();
        FindObjectOfType<PlayerMenuUI>().Show();
    }
}
