using UnityEngine;

public class PlataformBGM : MonoBehaviour
{
    private void Start() => StageManager.Instance.OnStageStart += SelectBGM;

    [SerializeField]
    private int numDay, numAfternoon, numNight;

    private void SelectBGM()
    {
        switch (Spawner.Instance.SpawnObjects)
        {
            case EnemyType.Targets:
                AudioManager.Instance.PlaySound($"BGM_Day{Random.Range(1, numDay)}");
                break;
            case EnemyType.TargetsAndObstacles:
                AudioManager.Instance.PlaySound($"BGM_Afternoon{Random.Range(1, numAfternoon)}");
                break;
            case EnemyType.Obstacles:
                AudioManager.Instance.PlaySound($"BGM_Night{Random.Range(1, numNight)}");
                break;
        }
    }
}
