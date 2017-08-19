using System;
using UnityEngine;

public class MainScreenManager : MonoBehaviour
{
    public GameObject Panel1, Panel2, PanelNewGame, PanelLoadGame;
    public LevelLoader LevelLoader;
    public GameObject[] ObjectsToHide;

    public void ButtonStartGame()
    {
        Panel1.SetActive(false);
        Panel2.SetActive(true);
    }

    public void ButtonNewGame()
    {
        PanelNewGame.SetActive(true);
        HideObjects();
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
}
