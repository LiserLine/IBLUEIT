using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private bool _helpPanelActive = false;
    private int _activeScene;

    [Header("Settings")]
    public int PreviousScene;
    public GameObject HelpPanel;

    private void Start()
    {
        _activeScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        ToggleShowHelp(); // F1
        ToggleResetStage(); // F2
        ToggleQuitOnEscape(); // ESC
    }

    private void ToggleShowHelp()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _helpPanelActive = !_helpPanelActive;
            HelpPanel.SetActive(_helpPanelActive);
        }
    }

    private void ToggleResetStage()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Reset Scene...");
            SceneManager.LoadScene(_activeScene, LoadSceneMode.Single);
        }
    }

    private void ToggleQuitOnEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(PreviousScene);
        }
    }
}
