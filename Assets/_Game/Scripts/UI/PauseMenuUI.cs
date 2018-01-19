public class PauseMenuUI : BasicUI<PauseMenuUI>
{
    public void PauseGame() => GameMaster.Instance.PauseGame();
}
