using NaughtyAttributes;
using UnityEngine;
using Ibit.Core.Game;
using Ibit.Plataform.Manager.Spawn;

namespace Ibit.Plataform.Manager.Score
{
    public enum GameResult
    {
        Failure,
        Success
    }

    public partial class Scorer : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private float maxScore;

        [SerializeField]
        [ReadOnly]
        private float score;

        public float MaxScore => maxScore;
        public GameResult Result { get; private set; }
        public float Score => score;

        public GameResult CalculateResult(bool gameOver = false)
        {
            if (gameOver)
            {
                Result = GameResult.Failure;
            }
            else
            {
                Result = Score >= MaxScore * GameManager.LevelUnlockScoreThreshold
                    ? GameResult.Success
                    : GameResult.Failure;
            }

            return Result;
        }

        private void Awake()
        {
            score = 0;
            FindObjectOfType<Spawner>().OnObjectReleased += MaxScoreUpdate;
            FindObjectOfType<Player>().OnEnemyHit += Player_OnEnemyHit;
        }

        private float CalculateObstacleScore(float size, float spawnDelay, float gameDifficulty) => size * (1f + (1f / spawnDelay)) * gameDifficulty * 100f;

        private float CalculateTargetScore(float height, float spawnDelay, float gameDifficulty) => Mathf.Abs(height) * (1f + (1f / spawnDelay)) * gameDifficulty * 100f;

        private void MaxScoreUpdate(ObjectToSpawn enemytype, ref GameObject go1, ref GameObject go2)
        {
            switch (enemytype)
            {
                case ObjectToSpawn.Targets:
                    maxScore += CalculateTargetScore(go1.transform.position.y, Data.Stage.Loaded.SpawnDelay, Data.Stage.Loaded.GameDifficulty);
                    maxScore += CalculateTargetScore(go2.transform.position.y, Data.Stage.Loaded.SpawnDelay, Data.Stage.Loaded.GameDifficulty);
                    break;

                case ObjectToSpawn.Obstacles:
                    maxScore += CalculateObstacleScore(go1.transform.localScale.x, Data.Stage.Loaded.SpawnDelay, Data.Stage.Loaded.GameDifficulty);
                    maxScore += CalculateObstacleScore(go2.transform.localScale.x, Data.Stage.Loaded.SpawnDelay, Data.Stage.Loaded.GameDifficulty);
                    break;
            }
        }
    }
}