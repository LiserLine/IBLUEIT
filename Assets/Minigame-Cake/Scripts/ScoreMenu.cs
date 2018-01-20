using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreMenu : MonoBehaviour
{
    public Stars[] finalScore = new Stars[3];

    private void Start() => gameObject.SetActive(false);

    public void DisplayFinalScore(int score1, int score2, int score3)
    {
        finalScore[0].FillStarsFinal(score1);
        finalScore[1].FillStarsFinal(score2);
        finalScore[2].FillStarsFinal(score3);
    }

    public void ToggleScoreMenu() => gameObject.SetActive(true);

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
