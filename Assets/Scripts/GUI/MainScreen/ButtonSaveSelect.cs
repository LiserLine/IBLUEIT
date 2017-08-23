using UnityEngine;

public class ButtonSaveSelect : MonoBehaviour
{
    public void LoadPlayerSave()
    {
        var player = GetComponent<PlayerHolder>().Player;
        GameManager.Instance.Player = player;
        Debug.Log($"{player.Name} save loaded.");

        var panelLoadGame = GameObject.Find("PanelLoadGame");
        panelLoadGame.SetActive(false);

        var panelMessage = GameObject.Find("PanelMessage");
        panelMessage.SendMessage("ShowMessage", "Daqui pra frente tem que aparecer a tela de escolher plataforma ou minigame");
    }
}

public class PlayerHolder : MonoBehaviour
{
    public Player Player;
}

