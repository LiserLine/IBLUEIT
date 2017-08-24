using System;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenManager : MonoBehaviour
{
    private GameObject _activePanel;

    public GameObject Panel1, Panel2, Panel3, PanelNewGame, PanelLoadGame;
    public PanelMessage PanelMessage;
    public LevelLoader LevelLoader;
    public GameObject[] ObjectsToHide;

    private void SwitchPanels(ref GameObject newPanel)
    {
        _activePanel?.SetActive(false);
        _activePanel = newPanel;
        newPanel.SetActive(true);
    }

    private void HideObjects()
    {
        foreach (var obj in ObjectsToHide)
        {
            obj.SetActive(false);
        }
    }

    private void ShowObjects()
    {
        foreach (var obj in ObjectsToHide)
        {
            obj.SetActive(true);
        }
    }

    public void GoToPanel2()
    {
        Panel1.SetActive(false);
        SwitchPanels(ref Panel2);
    }

    public void GoToNewGamePanel()
    {
        SwitchPanels(ref PanelNewGame);
        HideObjects();
    }

    public void GoToLoadGamePanel()
    {
        SwitchPanels(ref PanelLoadGame);
        HideObjects();
    }

    public void ShowGameInfo()
    {
        var credits = $"I Blue It - v0.0.0 (DEV)" +
                      $"\n[Renato Hartmann Grimes]" +
                      $"\nEletrical Engineering Department" +
                      $"\n[Marcelo da Silva Hounsell]" +
                      $"\nComputer Science Department" +
                      $"\n[Santa Catarina State University]" +
                      $"\nCenter of Technological Sciences";
        PanelMessage.ShowInfo(credits);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToPanel2()
    {
        GoToPanel2();
        ShowObjects();
    }

    public void GoToPanel3()
    {
        SwitchPanels(ref Panel3);
    }

    public void StarNewGame()
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
            var errMsg = LocalizationManager.Instance?.GetLocalizedValue("error_invalidDate");
            PanelMessage.ShowError(errMsg);
            return;
        }

        var playerName = GameObject.Find("InputFieldName").GetComponent<InputField>().text;

        if (playerName.Length == 0)
        {
            var errMsg = LocalizationManager.Instance?.GetLocalizedValue("error_undefinedPlayerName");
            PanelMessage.ShowError(errMsg);
            return;
        }

        var normal = GameObject.Find("ToggleNormal").GetComponent<Toggle>().isOn;
        var obstructive = GameObject.Find("ToggleObstructive").GetComponent<Toggle>().isOn;
        var restrictive = GameObject.Find("ToggleRestrictive").GetComponent<Toggle>().isOn;

        if (normal == obstructive == restrictive == false)
        {
            var errMsg = LocalizationManager.Instance?.GetLocalizedValue("error_undefinedDisfunction");
            PanelMessage.ShowError(errMsg);
            return;
        }

        var disfunction = restrictive ? Disfunctions.Restrictive
            : (obstructive ? Disfunctions.Obstructive : Disfunctions.Normal);

        var observations = GameObject.Find("Observations").GetComponent<InputField>().text;

        var plr = new Player
        {
            Name = playerName,
            Birthday = birthday,
            Disfunction = disfunction,
            Id = (uint)DatabaseManager.Instance.Players.PlayerList.Count + 1,
            Observations = observations
        };

        var tmpPlr = DatabaseManager.Instance.Players.GetPlayer(playerName);

        if (plr.Name.Equals(tmpPlr?.Name)
            && plr.Birthday.Equals(tmpPlr?.Birthday)
            && plr.Disfunction.Equals(tmpPlr?.Disfunction))
        {
            var errMsg = LocalizationManager.Instance?.GetLocalizedValue("error_alreadyExists");
            PanelMessage.ShowError(errMsg);
            return;
        }

        DatabaseManager.Instance.Players.CreatePlayer(plr);
        Debug.Log($"Save for {plr.Name} created!");
        GameManager.Instance.Player = plr;
        GoToPanel3();
    }

    public void LoadPlataform()
    {
        if (GameManager.Instance.Player.TutorialDone)
        {
            LevelLoader.LoadScene(2);
        }
        else
        {
            LevelLoader.LoadScene(3);
        }
    }

    public void ShowPlayerInfo()
    {
        var player = GameManager.Instance.Player;

        var txtBDay = LocalizationManager.Instance?.GetLocalizedValue("txtBDay");
        var txtDisfunction = LocalizationManager.Instance?.GetLocalizedValue("txtDisfunction");
        var txtScoreMax = LocalizationManager.Instance?.GetLocalizedValue("txtScoreMax");
        var txtSessions = LocalizationManager.Instance?.GetLocalizedValue("txtSessions");

        var plrInfo = $"{player?.Name}" +
                      $"\nID: {player?.Id}" +
                      $"\n{txtBDay}: {player?.Birthday:dd/MM/yyyy}" +
                      $"\n{txtDisfunction}: {player?.Disfunction}" +
                      $"\n{txtScoreMax}: {player?.TotalScore}" +
                      $"\n{txtSessions}: {player?.SessionsDone}";

        PanelMessage.ShowInfo(plrInfo);
    }
}
