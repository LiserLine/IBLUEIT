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
            maxScore += CalculateTargetScore(go1.transform.position.y, spawner.spawnDelay, spawner.gameDifficulty);
            maxScore += CalculateTargetScore(go2.transform.position.y, spawner.spawnDelay, spawner.gameDifficulty);
        }
        else if(enemytype == EnemyType.Targets)
        {
            maxScore += CalculateObstacleScore(go1.transform.localScale.x, spawner.spawnDelay, spawner.gameDifficulty);
            maxScore += CalculateObstacleScore(go2.transform.localScale.x, spawner.spawnDelay, spawner.gameDifficulty);
        }
    }

    private void Player_OnEnemyHit(GameObject hit)
    {
        if (hit.tag.Contains("Target"))
        {
            score += CalculateTargetScore(hit.transform.position.y, spawner.spawnDelay, spawner.gameDifficulty);
        }
    }

    private float CalculateTargetScore(float height, float spawnDelay, float gameDifficulty)
    {
        return Mathf.Abs(height) * (1f + (1f / spawnDelay)) * gameDifficulty;
    }

    private float CalculateObstacleScore(float size, float spawnDelay, float gameDifficulty)
    {
        return size * (1f + (1f / spawnDelay)) * gameDifficulty;
    }
}
