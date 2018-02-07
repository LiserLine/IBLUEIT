﻿using NaughtyAttributes;
using UnityEngine;

public enum GameResult
{
    Failure = 0,
    Success = 1
}

public partial class Scorer : Singleton<Scorer>
{
    public float Score => score;
    public float MaxScore => maxScore;

    [SerializeField]
    [ReadOnly]
    private float score;

    [SerializeField]
    [ReadOnly]
    private float maxScore;

    public delegate void ResultCalculatedHandler(GameResult result);
    public event ResultCalculatedHandler OnResultCalculated;

    private void OnEnable()
    {
        score = 0;
        Player.Instance.OnEnemyHit += Player_OnEnemyHit;
        Spawner.Instance.OnObjectReleased += MaxScoreUpdate;
        StageManager.Instance.OnStageEnd += CalculateResult;
    }

    private void CalculateResult()
    {
        var result = score >= maxScore * GameMaster.PlataformMinScoreMultiplier
            ? GameResult.Success
            : GameResult.Failure;

        if (result == GameResult.Success)
        {
            Pacient.Loaded.StagesOpened++;
        }
        else
        {
            if(score < maxScore * 0.3f)
                Pacient.Loaded.StagesOpened--;

            if (Pacient.Loaded.StagesOpened == 0)
                Pacient.Loaded.StagesOpened = 1;
        }

        OnResultCalculated?.Invoke(result);
    }

    private void MaxScoreUpdate(EnemyType enemytype, ref GameObject go1, ref GameObject go2)
    {
        if (enemytype == EnemyType.Targets)
        {
            maxScore += CalculateTargetScore(go1.transform.position.y, Spawner.Instance.SpawnDelay, Spawner.Instance.GameDifficulty);
            maxScore += CalculateTargetScore(go2.transform.position.y, Spawner.Instance.SpawnDelay, Spawner.Instance.GameDifficulty);
        }
        else if (enemytype == EnemyType.Obstacles)
        {
            maxScore += CalculateObstacleScore(go1.transform.localScale.x, Spawner.Instance.SpawnDelay, Spawner.Instance.GameDifficulty);
            maxScore += CalculateObstacleScore(go2.transform.localScale.x, Spawner.Instance.SpawnDelay, Spawner.Instance.GameDifficulty);
        }
    }

    private void Player_OnEnemyHit(GameObject hit)
    {
        if (hit.tag.Equals("AirTarget") || hit.tag.Equals("WaterTarget") || hit.tag.Equals("RelaxCoin"))
            score += CalculateTargetScore(hit.transform.position.y, Spawner.Instance.SpawnDelay, Spawner.Instance.GameDifficulty);
    }

    private float CalculateTargetScore(float height, float SpawnDelay, float GameDifficulty) => Mathf.Abs(height) * (1f + (1f / SpawnDelay)) * GameDifficulty;

    private float CalculateObstacleScore(float size, float SpawnDelay, float GameDifficulty) => size * (1f + (1f / SpawnDelay)) * GameDifficulty;
}
