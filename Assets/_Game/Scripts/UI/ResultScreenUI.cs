using UnityEngine;
using UnityEngine.UI;

//ToDo - separar em text game objects e dividir em textos para auxiliar na programaçao do localization system
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
            $"Score: {Scorer.Instance.Score} / {Scorer.Instance.MaxScore} ({Mathf.Round(Scorer.Instance.Score / Scorer.Instance.MaxScore * 100f)}%)\n" +
            $"Fase: {Spawner.StageToLoad}\n" +
            $"Nível Altura: {Spawner.Instance.InspiratoryHeightLevel}\n" +
            $"Nível Profundidade: {Spawner.Instance.ExpiratoryHeightLevel}\n" +
            $"Nível Tamanho: {Spawner.Instance.ExpiratorySizeLevel}\n" +
            $"Jogador: {Player.Data.Name} ({Player.Data.Id})";

        base.Show();

        GameManager.Instance.PauseGame();
    }
}
