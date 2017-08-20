using System;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenManager : MonoBehaviour
{
    public GameObject PanelMessage, Panel1, Panel2, PanelNewGame, PanelLoadGame;
    public LevelLoader LevelLoader;
    public GameObject[] ObjectsToHide;

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

    public void ButtonStartGame()
    {
        Panel1.SetActive(false);
        Panel2.SetActive(true);
    }

    public void ButtonNewGame()
    {
        Panel2.SetActive(false);
        PanelNewGame.SetActive(true);
        HideObjects();
    }

    public void ButtonLoadGame()
    {
        Panel2.SetActive(false);
        PanelLoadGame.SetActive(true);
        HideObjects();
    }

    public void ButtonInfoGame()
    {
        throw new Exception("Not yet implemented.");
    }

    public void ButtonQuitGame()
    {
        Application.Quit();
    }

    public void ReturnToNewLoadPanel()
    {
        PanelNewGame.SetActive(false);
        PanelLoadGame.SetActive(false);
        Panel2.SetActive(true);
        ShowObjects();
    }

    public void StarNewGame()
    {
        var playerName = GameObject.Find("InputFieldName").GetComponent<InputField>().text;

        var bDay = GameObject.Find("LabelBDay").GetComponent<Text>().text;
        var bMonth = GameObject.Find("LabelBMonth").GetComponent<Text>().text;
        var bYear = GameObject.Find("LabelBYear").GetComponent<Text>().text;
        var birthday = new DateTime(int.Parse(bYear), int.Parse(bMonth), int.Parse(bDay));

        var normal = GameObject.Find("ToggleNormal").GetComponent<Toggle>().isOn;
        var obstructive = GameObject.Find("ToggleObstructive").GetComponent<Toggle>().isOn;
        var restrictive = GameObject.Find("ToggleRestrictive").GetComponent<Toggle>().isOn;

        var observations = GameObject.Find("Observations").GetComponent<InputField>().text;

        //ToDo - wrong date?
        //ToDo - repeated user?

        PanelMessage.SendMessage("ShowError", "Not yet implemented");
    }
}
