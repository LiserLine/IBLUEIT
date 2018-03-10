using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ibit.CakeGame
{
    public class ScoreMenu : MonoBehaviour
    {
        public Stars[] finalScore = new Stars[3];
        public Text[] peakText = new Text[3];
        public string[] pikeString = new string[3];

        public void DisplayFinalScore(int score1, int score2, int score3)
        {
            finalScore[0].FillStarsFinal(score1);
            finalScore[1].FillStarsFinal(score2);
            finalScore[2].FillStarsFinal(score3);
            peakText[0].text = "Pico: " + pikeString[0];
            peakText[1].text = "Pico: " + pikeString[1];
            peakText[2].text = "Pico: " + pikeString[2];
        }

        public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        public void ToggleScoreMenu() => gameObject.SetActive(true);

        private void Start() => gameObject.SetActive(false);
    }
}