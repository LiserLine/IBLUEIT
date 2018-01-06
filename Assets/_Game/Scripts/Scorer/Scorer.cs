using UnityEngine;

public partial class Scorer : MonoBehaviour
{
    [SerializeField]
    private Spawner spawner;

    [SerializeField]
    private float score;

    private void OnEnable()
    {
        score = 0;
        Player.OnEnemyHit += Player_OnEnemyHit;
    }

    private void Player_OnEnemyHit(GameObject hit)
    {
        if (hit.tag.Contains("Target"))
        {
            score += Mathf.Abs(hit.transform.position.y) * (1f + (1f / spawner.spawnDelay)) * spawner.gameDifficulty;
        }
    }

    private void OnDisable()
    {
        Player.OnEnemyHit -= Player_OnEnemyHit;
    }
}
