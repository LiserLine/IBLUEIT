using UnityEngine;

public class SaveDataButtonFunc : MonoBehaviour
{
    public void LoadPlayerSave()
    {
        var player = GetComponent<PlayerDataHolder>().Pacient;
        Pacient.Loaded = player;
        Debug.Log($"{player.Name} save loaded.");
        FindObjectOfType<LoadGameMenuUI>().Hide();
        FindObjectOfType<PlayerMenuUI>().Show();
    }
}

public class PlayerDataHolder : MonoBehaviour
{
    public Pacient Pacient;
}
