using UnityEngine;

public partial class Scorer
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AirTarget") || collision.gameObject.CompareTag("WaterTarget") ||
            collision.gameObject.CompareTag("RelaxCoin"))
        {
            FindObjectOfType<Spawner>().Player_OnEnemyMiss(collision.gameObject.tag);
            return;
        }

        if (!collision.gameObject.CompareTag("AirObstacle") && !collision.gameObject.CompareTag("WaterObstacle"))
            return;

        if (collision.gameObject.GetComponent<Obstacle>().HeartPoint <= 0)
            return;

        score += CalculateObstacleScore(collision.gameObject.transform.localScale.x, Stage.Loaded.SpawnDelay, Stage.Loaded.GameDifficulty);
    }

    private void Player_OnEnemyHit(GameObject hit)
    {
        if (hit.CompareTag("AirTarget") || hit.CompareTag("WaterTarget") || hit.CompareTag("RelaxCoin"))
            score += CalculateTargetScore(hit.transform.position.y, Stage.Loaded.SpawnDelay, Stage.Loaded.GameDifficulty);
    }
}