using UnityEngine;

public class ButtonSaveSelect : MonoBehaviour
{
    public void LoadPlayerSave()
    {
        var player = GetComponent<PlayerHolder>().Player;
        GameManager.Instance.Player = player;
        Debug.Log($"{player.Name} save loaded.");

        var canvas = GameObject.Find("Canvas");
        canvas.SendMessage("GoToPanel3");
    }
}

public class PlayerHolder : MonoBehaviour
{
    public Player Player;
}

