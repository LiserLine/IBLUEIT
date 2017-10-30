using UnityEngine;

public class MenuButtonsOnClick : MonoBehaviour
{
    public GameObject menuPanel, gamePanel, gameElements, serialCommPrefab;

    public void LoadPhase(int stageId)
    {
        
        var sComm = Instantiate(serialCommPrefab);
        GameManager.Instance.Stage = new Stage
        {
            Id = stageId,
        };
        //sComm.GetComponent<SerialController>().OnSerialMessageReceived += OnSerialMessageReceived;

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameElements.SetActive(true);
    }

    public void LoadMenu()
    {

        var sComm = GameObject.Find("SerialController");
        //sComm.GetComponent<SerialController>().OnSerialMessageReceived -= OnSerialMessageReceived;
        Destroy(sComm);

        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameElements.SetActive(false);
    }

    //private void OnSerialMessageReceived(string msg)
    //{

    //}
}
