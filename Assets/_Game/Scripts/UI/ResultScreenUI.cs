using UnityEngine;
using UnityEngine.UI;

public class ResultScreenUI : GenericUI<ResultScreenUI>
{
    [SerializeField]
    private Text finalResult, motivationText, resultInfo;

    private void Awake() => Scorer.Instance.OnResultCalculated += Show;

    private void Show(GameResult result)
    {
        if (result == GameResult.Success)
        {
            finalResult.text = "GoGoGo!";
            motivationText.text = "Muito bem! Você passou de fase. Continue assim!";
            AudioManager.Instance.PlaySound("StageClear");
        }
        else
        {
            finalResult.text = "YOU BLEW IT";
            motivationText.text = "Você não conseguiu pontos suficientes. Não desista!";
            AudioManager.Instance.PlaySound("PlayerDamage");
        }

        resultInfo.text =
            $"• Score: {Mathf.Round(Scorer.Instance.Score)} / {Mathf.Round(Scorer.Instance.MaxScore)} ({Mathf.Round(Scorer.Instance.Score / Scorer.Instance.MaxScore * 100f)}%)\n" +
            $"• Fase: {Spawner.StageToLoad}\n" +
            $"• Nível Altura: {Spawner.Instance.InspiratoryHeightLevel}\n" +
            $"• Nível Profundidade: {Spawner.Instance.ExpiratoryHeightLevel}\n" +
            $"• Nível Tamanho: {Spawner.Instance.ExpiratorySizeLevel}\n" +
            $"• Jogador: {PlayerData.Player.Name} ({PlayerData.Player.Id})";

        base.Show();

        GameMaster.Instance.PauseGame();
    }
}
