using NaughtyAttributes;
using UnityEngine;

public enum EnemyType //ToDo - move from here
{
    Targets,
    TargetsAndObstacles,
    Obstacles
}

public partial class Spawner : MonoBehaviour
{
    [BoxGroup("Stage Settings")]
    public EnemyType spawnObjects;

    [BoxGroup("Stage Settings")]
    [Tooltip("Delay between spawned objects.")]
    public float spawnDelay = 5;

    [BoxGroup("Stage Settings")]
    [Dropdown("gameDifficulties")]
    public float gameDifficulty;

    private float[] gameDifficulties = { 40f, 50f, 60f, 70f, 80f, 90f, 100f };

    [BoxGroup("Stage Settings")]
    [Slider(1f, 5f)]
    public float objectSpeed = 1;

    [BoxGroup("Targets")]
    [Slider(1f, 5f)]
    public float distanceBetweenTargets = 2f;

    [BoxGroup("Targets")]
    [Slider(5f, 15f)]
    public float heightIncrement = 10f;

    [BoxGroup("Targets")]
    [Slider(1, 10)]
    public int targetThresholdLevelUp = 3;

    [BoxGroup("Targets")]
    [Slider(1, 10)]
    public int targetThresholdLevelDown = 2;

    [BoxGroup("Targets")]
    public GameObject[] targetsAir, targetsWater;

    [BoxGroup("Obstacles")]
    [Slider(1f, 5f)]
    public float distanceBetweenObstacles = 3f;

    [BoxGroup("Obstacles")]
    [Slider(5f, 15f)]
    public float sizeIncrement = 10f;

    [BoxGroup("Obstacles")]
    [Slider(1, 10)]
    public int obstacleThresholdLevelUp = 3;

    [BoxGroup("Obstacles")]
    [Slider(1, 10)]
    public int obstacleThresholdLevelDown = 2;

    [BoxGroup("Obstacles")]
    public GameObject[] obstaclesAir, obstaclesWater;

    private float timer;
    private bool spawnEnabled;
    private float maximumScore;

    private void OnEnable()
    {
        StageManager.OnStageStart += EnableSpawn;
        StageManager.OnStageEnd += DisableSpawn;
        Player.OnPlayerDeath += DisableSpawn;
        Player.OnEnemyHit += Player_OnEnemyHit;
        Scorer.OnEnemyMiss += Player_OnEnemyMiss;
    }

    private void OnDisable()
    {
        StageManager.OnStageStart -= EnableSpawn;
        StageManager.OnStageEnd -= DisableSpawn;
        Player.OnPlayerDeath -= DisableSpawn;
        Player.OnEnemyHit -= Player_OnEnemyHit;
        Scorer.OnEnemyMiss -= Player_OnEnemyMiss;
    }

    [Button("Enable Spawn")]
    private void EnableSpawn()
    {
        if (spawnEnabled)
            return;

        spawnEnabled = true;
        timer = spawnDelay;
    }

    [Button("Disable Spawn")]
    private void DisableSpawn()
    {
        if (!spawnEnabled)
            return;

        spawnEnabled = false;
        timer = 0f;
    }

    public void Update()
    {
        if (!spawnEnabled)
            return;

        timer += Time.deltaTime;

        if (timer > spawnDelay)
        {
            timer = 0f;
            Release();
        }
    }
}
