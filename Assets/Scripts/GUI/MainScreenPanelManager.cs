using UnityEngine;

public class MainScreenPanelManager : MonoBehaviour
{
    public GameObject Panel1, Panel2;

    public void ButtonStartGame()
    {
        Panel1.SetActive(false);
        Panel2.SetActive(true);
    }

    public void ButtonNewGame()
    {

    }

    public void ButtonLoadGame()
    {

    }
}
