using UnityEngine;

public class MenuButtonsOnClick : MonoBehaviour
{
    public GameObject menuPanel, gamePanel, gameElements, serialCommPrefab;
    public PlataformSceneManager sceneManager;

    private void OnEnable()
    {
        GameManager.Instance.Stage.OnStageEnd += LoadMenu;
    }

    private void OnDisable()
    {
        GameManager.Instance.Stage.OnStageEnd -= LoadMenu;
    }

    public void LoadStage(int stageId)
    {
        Instantiate(serialCommPrefab);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameElements.SetActive(true);

        sceneManager.StartStage(stageId);
    }

    public void LoadMenu()
    {
        sceneManager.EndStage();
        gameElements.SetActive(false);
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
    }
}
