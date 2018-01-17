using UnityEngine;

public partial class GameMaster
{
    public delegate void GamePauseHandler();
    public event GamePauseHandler OnGamePause;

    public delegate void GameUnPauseHandler();
    public event GameUnPauseHandler OnGameUnPause;

    public bool GameIsPaused { get; private set; }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        OnGamePause?.Invoke();
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        OnGameUnPause?.Invoke();
    }
}
