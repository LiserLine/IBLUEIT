using Ibit.Core.Game;
using Ibit.Plataform.Data;
using NaughtyAttributes;
using UnityEngine;

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
            maxScore = 0;
            FindObjectOfType<Player>().OnObjectHit += ScoreOnPlayerCollision;
        }

        private float CalculateObstacleScore(float size, float difficultyFactor)
        {
            return size * (1f + difficultyFactor) * 100f;
        }

        private float CalculateTargetScore(float height, float difficultyFactor)
        {
            return Mathf.Abs(height) * (1f + difficultyFactor) * 100f;
        }

        public void UpdateMaxScore(StageObjectType type, ref GameObject obj, float difficultyFactor)
        {
            switch (type)
            {
                case StageObjectType.Target:
                    maxScore += CalculateTargetScore(obj.transform.position.y, difficultyFactor);
                    break;

                case StageObjectType.Obstacle:
                    maxScore += CalculateObstacleScore(obj.transform.localScale.x, difficultyFactor);
                    break;
            }
        }
    }
}