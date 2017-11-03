using UnityEngine;

public class MenuButtonsOnClick : MonoBehaviour
{
    public GameObject menuPanel, waterPanel, gamePanel, gameElements, serialCommPrefab;
    public PlataformSceneManager sceneManager;

    public void LoadStage(int stageId)
    {
        Instantiate(serialCommPrefab);
        GameManager.Instance.Stage = new PlataformStage { Id = stageId };
        GameManager.Instance.Stage.OnStageEnd += LoadMenu;
        GameManager.Instance.Stage.Start();
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        waterPanel.SetActive(true);
        gameElements.SetActive(true);
    }

    public void LoadMenu()
    {
        GameManager.Instance.Stage.OnStageEnd -= LoadMenu;
        GameManager.Instance.Stage.Stop();
        gameElements.SetActive(false);
        waterPanel.SetActive(false);
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
    }
}
