using System;
using UnityEngine;

public class MainScreenManager : MonoBehaviour
{
    public GameObject Panel1, Panel2, PanelNewGame, PanelLoadGame;
    public LevelLoader LevelLoader;

    public void ButtonStartGame()
    {
        Panel1.SetActive(false);
        Panel2.SetActive(true);
    }

    public void ButtonNewGame()
    {
        throw new Exception("Not yet implemented.");
    }

    public void ButtonLoadGame()
    {
        throw new Exception("Not yet implemented.");
    }

    public void ButtonInfoGame()
    {
        throw new Exception("Not yet implemented.");
    }

    public void ButtonQuitGame()
    {
        Application.Quit();
    }
}
