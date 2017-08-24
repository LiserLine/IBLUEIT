using UnityEngine;

public class ButtonSaveSelect : MonoBehaviour
{
    public void LoadPlayerSave()
    {
        var player = GetComponent<PlayerHolder>().Player;
        GameManager.Instance.Player = player;
        Debug.Log($"{player.Name} save loaded.");
        GameObject.Find("Canvas").GetComponent<MainScreenManager>().GoToPanel3();
    }
}

public class PlayerHolder : MonoBehaviour
{
    public Player Player;
}

