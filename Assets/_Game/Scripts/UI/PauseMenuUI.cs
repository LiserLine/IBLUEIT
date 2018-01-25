public class PauseMenuUI : BasicUI<PauseMenuUI>
{
    public void PauseGame()
    {
        if (!StageManager.Instance.IsRunning)
            return;

        GameMaster.Instance.PauseGame();
    }

    public void UnPauseGame() => GameMaster.Instance.UnPauseGame();
}
