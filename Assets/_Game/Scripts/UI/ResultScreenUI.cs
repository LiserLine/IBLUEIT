using UnityEngine;
using UnityEngine.UI;

//ToDo - separar em text game objects e dividir em textos para auxiliar na programaçao do localization system
public class ResultScreenUI : MonoBehaviour
{
    [SerializeField]
    private Text finalResult, motivationText, resultInfo;

    private void Awake() => Scorer.Instance.OnResultCalculated += ShowResultScreen;

    private void ShowResultScreen(GameResult result)
    {
        Time.timeScale = 0f;

        if (result == GameResult.Success)
        {
            finalResult.text = "GoGoGo!";
            motivationText.text = "Muito bem! Continue assim!";
        }
        else
        {
            finalResult.text = "YOU BLEW IT";
            motivationText.text = "Você não conseguiu pontos suficientes. Não desista!";
        }

        resultInfo.text =
            $"Score: {Scorer.Instance.Score} / {Scorer.Instance.MaxScore} ({Mathf.Round(Scorer.Instance.Score / Scorer.Instance.MaxScore * 100f)}%)\n" +
            $"Fase: {Spawner.StageToLoad}\n" +
            $"Nível Altura: {Spawner.Instance.InspiratoryHeightLevel}\n" +
            $"Nível Profundidade: {Spawner.Instance.ExpiratoryHeightLevel}\n" +
            $"Nível Tamanho: {Spawner.Instance.ExpiratorySizeLevel}\n" +
            $"Jogador: {Player.Data.Name} ({Player.Data.Id})";

        this.transform.localScale = Vector3.one;
    }
}
