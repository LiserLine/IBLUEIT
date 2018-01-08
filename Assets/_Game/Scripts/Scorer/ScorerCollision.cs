using UnityEngine;

public partial class Scorer
{
    public delegate void EnemyMissedHandler(GameObject miss);
    public static event EnemyMissedHandler OnEnemyMiss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("AirTarget") || collision.gameObject.tag.Equals("WaterTarget"))
        {
            OnEnemyMiss?.Invoke(collision.gameObject);
        }
        else if (collision.gameObject.tag.Equals("AirObstacle") || collision.gameObject.tag.Equals("WaterObstacle"))
        {
            var hp = collision.gameObject.GetComponent<Obstacle>().HeartPoint;
            if (hp <= 0)
                return;

            OnEnemyMiss?.Invoke(collision.gameObject); //avoided object
            score += CalculateObstacleScore(collision.gameObject.transform.localScale.x, spawner.SpawnDelay, spawner.GameDifficulty);
        }
    }
}
