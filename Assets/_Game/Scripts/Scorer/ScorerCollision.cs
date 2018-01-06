using UnityEngine;

public partial class Scorer
{ 
    public delegate void EnemyMissedHandler(GameObject miss);
    public static event EnemyMissedHandler OnEnemyMiss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Target"))
        {
            OnEnemyMiss?.Invoke(collision.gameObject);
        }
        else if (collision.gameObject.tag.Contains("Obstacle"))
        {
            var hp = collision.gameObject.GetComponent<Obstacle>().HeartPoint;
            if (hp <= 0)
                return;

            OnEnemyMiss?.Invoke(collision.gameObject); //avoided object
            score += collision.gameObject.transform.localScale.x * (1f + (1f / spawner.spawnDelay)) * spawner.gameDifficulty;
        }
    }
}
