using UnityEngine;

public class SaveDataButtonFunc : MonoBehaviour
{
    public void LoadPlayerSave()
    {
        var player = GetComponent<PlayerDataHolder>().PlayerData;
        PlayerData.Player = player;
        Debug.Log($"{player.Name} save loaded.");
        FindObjectOfType<LoadGameMenuUI>().Hide();
        FindObjectOfType<PlayerMenuUI>().Show();
    }
}

public class PlayerDataHolder : MonoBehaviour
{
    public PlayerData PlayerData;
}
