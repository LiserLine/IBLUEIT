using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class ResultScreenUI : MonoBehaviour
{
    [SerializeField]
    private Text finalResult, motivationText, resultInfo;

    [SerializeField]
    private Button nextStageButton;

    private void OnEnable()
    {
        var scorer = FindObjectOfType<Scorer>();

        if (scorer.Result == GameResult.Success)
        {
            finalResult.text = "GoGoGo!";
            finalResult.color = Color.cyan;
            motivationText.text = "Muito bem! Você passou de fase. Continue assim!";
            SoundManager.Instance.PlaySound("StageClear");
        }
        else
        {
            nextStageButton.interactable = false;
            finalResult.text = "YOU BLEW IT";
            finalResult.color = Color.red;
            motivationText.text = "Você não conseguiu pontos suficientes. Não desista!";
            SoundManager.Instance.PlaySound("PlayerDamage");
        }

        var score = scorer.Score;
        var maxScore = scorer.MaxScore;
        score = Mathf.Clamp(score, 0f, maxScore);

        resultInfo.text =
            $"• Score: {score:####} / {maxScore:####} ({((score / maxScore) * 100f):####}%)\n" +
            $"• Fase: {(int)Stage.Loaded.SpawnObject}\n" +
            $"• Nível: {Stage.Loaded.Level}\n" +
            $"• Jogador: {Pacient.Loaded.Name}";
    }
}
