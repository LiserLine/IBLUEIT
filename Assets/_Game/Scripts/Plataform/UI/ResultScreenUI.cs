using UnityEngine;
using UnityEngine.UI;
using Ibit.Core.Data;
using Ibit.Core.Audio;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Score;
using Color = UnityEngine.Color;

namespace Ibit.Plataform.UI
{
    public class ResultScreenUI : MonoBehaviour
    {
        [SerializeField]
        private Text finalResult, motivationText, resultInfo;

        [SerializeField]
        private Button nextStageButton;

        [SerializeField]
        private Button pauseButton;

        [SerializeField]
        private Text scoreValue, scoreRatio;

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

            pauseButton.interactable = false;
            scoreValue.text = scoreRatio.text = "";

            resultInfo.text =
                $"• Score: {score:####} / {maxScore:####} ({((score / maxScore) * 100f):####}%)\n" +
                $"• Fase: {(int)Stage.Loaded.ObjectToSpawn}\n" +
                $"• Nível: {Stage.Loaded.Level}\n" +
                $"• Jogador: {Pacient.Loaded.Name}";
        }
    }
}