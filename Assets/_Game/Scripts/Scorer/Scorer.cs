using UnityEngine;

public partial class Scorer : MonoBehaviour
{
    [SerializeField]
    private Spawner spawner;

    [SerializeField]
    private float score;

    [SerializeField]
    private float maxScore;

    private void OnEnable()
    {
        score = 0;
        Player.OnEnemyHit += Player_OnEnemyHit;
        Spawner.OnObjectReleased += MaxScoreUpdate;
        StageManager.OnStageEnd += CalculateResult;
    }

    //ToDo - remove this from here
    private void CalculateResult()
    {
        Debug.Log(score >= maxScore * 0.7f ? "Stage completed!" : "Stage Failed!");
    }

    private void OnDisable()
    {
        Player.OnEnemyHit -= Player_OnEnemyHit;
        Spawner.OnObjectReleased -= MaxScoreUpdate;
    }

    private void MaxScoreUpdate(EnemyType enemytype, ref GameObject go1, ref GameObject go2)
    {
        if (enemytype == EnemyType.Targets)
        {
            maxScore += CalculateTargetScore(go1.transform.position.y, spawner.SpawnDelay, spawner.GameDifficulty);
            maxScore += CalculateTargetScore(go2.transform.position.y, spawner.SpawnDelay, spawner.GameDifficulty);
        }
        else if (enemytype == EnemyType.Obstacles)
        {
            maxScore += CalculateObstacleScore(go1.transform.localScale.x, spawner.SpawnDelay, spawner.GameDifficulty);
            maxScore += CalculateObstacleScore(go2.transform.localScale.x, spawner.SpawnDelay, spawner.GameDifficulty);
        }
    }

    private void Player_OnEnemyHit(GameObject hit)
    {
        if (hit.tag.Equals("AirTarget") || hit.tag.Equals("WaterTarget") || hit.tag.Equals("RelaxCoin"))
        {
            score += CalculateTargetScore(hit.transform.position.y, spawner.SpawnDelay, spawner.GameDifficulty);
        }
    }

    private float CalculateTargetScore(float height, float SpawnDelay, float GameDifficulty)
    {
        return Mathf.Abs(height) * (1f + (1f / SpawnDelay)) * GameDifficulty;
    }

    private float CalculateObstacleScore(float size, float SpawnDelay, float GameDifficulty)
    {
        return size * (1f + (1f / SpawnDelay)) * GameDifficulty;
    }
}
