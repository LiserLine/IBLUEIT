using UnityEngine;

public class MenuButtonsOnClick : MonoBehaviour
{
    public GameObject menuPanel, gamePanel, gameElements, serialCommPrefab;

    public void LoadStage(int stageId)
    {
        Instantiate(serialCommPrefab);
        GameManager.Instance.Stage = new Stage
        {
            Id = stageId,
        };

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameElements.SetActive(true);
    }

    public void LoadMenu()
    {
        var sComm = GameObject.Find("SerialController");
        Destroy(sComm);

        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameElements.SetActive(false);
    }

    //private void OnSerialMessageReceived(string msg)
    //{

    //}
}
