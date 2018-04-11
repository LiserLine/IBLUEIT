using Ibit.Plataform.Manager.Spawn;
using UnityEngine;

namespace Ibit.Plataform.Manager.Score
{
    public partial class Scorer
    {
        /// <summary>
        /// Obstacle Scoring
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("AirTarget") || collision.gameObject.CompareTag("WaterTarget"))
            {
                FindObjectOfType<Spawner>().PerformanceOnMiss(collision.gameObject.tag);
                return;
            }

            if (!collision.gameObject.CompareTag("AirObstacle") && !collision.gameObject.CompareTag("WaterObstacle"))
                return;

            if (collision.gameObject.GetComponent<Obstacle>().HeartPoint < 1)
                return;

            FindObjectOfType<Spawner>().PerformanceOnMiss(collision.gameObject.tag);
            score += CalculateObstacleScore(collision.gameObject.transform.localScale.x, collision.GetComponent<Obstacle>().Properties.DifficultyFactor);
        }

        /// <summary>
        /// Target Scoring
        /// </summary>
        /// <param name="hit"></param>
        private void ScoreOnPlayerCollision(GameObject hit)
        {
            if (hit.CompareTag("AirTarget") || hit.CompareTag("WaterTarget"))
                score += CalculateTargetScore(hit.transform.position.y, hit.GetComponent<Target>().Properties.DifficultyFactor);
        }
    }
}