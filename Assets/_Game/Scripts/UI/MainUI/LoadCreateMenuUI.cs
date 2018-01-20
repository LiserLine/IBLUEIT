public class LoadCreateMenuUI : BasicUI<LoadCreateMenuUI>
{
    private void Start()
    {
        if (PlayerData.Player == null)
            return;

        Hide();
        FindObjectOfType<PlayerMenuUI>().Show();
    }

    public void QuitGame() => GameMaster.Instance.QuitGame();
}
