using System;
using UnityEngine;

public partial class StageManager
{
    public delegate void PauseStageHandler();
    public event PauseStageHandler OnPauseStage;

    public delegate void ResumeStageHandler();
    public event ResumeStageHandler OnResumeStage;

    [SerializeField]
    private GameObject pauseMenuUI;

    public bool GameIsPaused { get; private set; }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        OnPauseStage?.Invoke();
        GameIsPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        OnResumeStage?.Invoke();
        GameIsPaused = false;
    }

    public void ReturnMenu()
    {
        //Time.timeScale = 1f;
        throw new NotImplementedException();
    }
}
