using UnityEngine;
using UnityEngine.UI;

public class GameScoreUI : MonoBehaviour
{
    private Scorer scorer;

    private void Awake() => scorer = FindObjectOfType<Scorer>();

    private void Update() => GetComponent<Text>().text = $"{scorer.Score:####}";
}