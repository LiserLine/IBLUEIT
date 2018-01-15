using UnityEngine;
using UnityEngine.UI;

public class GameScoreUI : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;

    private void FixedUpdate() => scoreText.text = Scorer.Instance.Score.ToString();
}
