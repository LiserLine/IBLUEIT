using UnityEngine;

public class MenuButtonsOnClick : MonoBehaviour
{
    public GameObject menuPanel, waterPanel, gamePanel, gameElements, serialCommPrefab, buttonReturnMain, buttonReturnMenu, buttonQuit;
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
        buttonReturnMain.SetActive(false);
        buttonReturnMenu.SetActive(true);
        buttonQuit.SetActive(false);
        gameElements.SetActive(true);
    }

    public void LoadMenu()
    {
        GameManager.Instance.Stage.OnStageEnd -= LoadMenu;
        gameElements.SetActive(false);
        waterPanel.SetActive(false);
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        buttonReturnMain.SetActive(true);
        buttonQuit.SetActive(true);
        buttonReturnMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
