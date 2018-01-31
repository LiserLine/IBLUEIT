using UnityEngine;
using UnityEngine.UI;

public class NextStageButtonUI : MonoBehaviour
{
    private void Awake() => Scorer.Instance.OnResultCalculated += OnResultCalculated;
    private void OnResultCalculated(GameResult result) => this.GetComponent<Button>().interactable = result == GameResult.Success;

    public void SetNextStage() => Spawner.StageToLoad++;
}