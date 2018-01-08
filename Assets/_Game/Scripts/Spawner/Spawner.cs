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
    public EnemyType SpawnObjects => spawnObjects;
    public float SpawnDelay => spawnDelay;
    public float GameDifficulty => gameDifficulty;

    [BoxGroup("Stage Settings")]
    [SerializeField]
    private EnemyType spawnObjects;

    [BoxGroup("Stage Settings")]
    [SerializeField]
    [Tooltip("Delay between spawned objects.")]
    private float spawnDelay = 5;

    [BoxGroup("Stage Settings")]
    [Dropdown("gameDifficulties")]
    [SerializeField]
    private float gameDifficulty = 40f;

    private float[] gameDifficulties = { 40f, 50f, 60f, 70f, 80f, 90f, 100f };

    [BoxGroup("Stage Settings")]
    [Slider(1f, 5f)]
    [SerializeField]
    private float objectSpeed = 1;

    [BoxGroup("Targets")]
    [Slider(1f, 5f)]
    [SerializeField]
    private float distanceBetweenTargets = 1.5f;

    [BoxGroup("Targets")]
    [Slider(5f, 15f)]
    [SerializeField]
    private float heightIncrement = 10f;

    [BoxGroup("Targets")]
    [SerializeField]
    [Slider(1, 10)]
    private int targetThresholdLevelUp = 3;

    [BoxGroup("Targets")]
    [SerializeField]
    [Slider(1, 10)]
    private int targetThresholdLevelDown = 2;

    [BoxGroup("Targets")]
    [SerializeField]
    private GameObject[] targetsAir;

    [BoxGroup("Targets")]
    [SerializeField]
    private GameObject[] targetsWater;

    [BoxGroup("Obstacles")]
    [SerializeField]
    [Slider(1f, 5f)]
    private float distanceBetweenObstacles = 3f;

    [BoxGroup("Obstacles")]
    [SerializeField]
    [Slider(5f, 30f)]
    private float sizeIncrement = 10f;

    [BoxGroup("Obstacles")]
    [SerializeField]
    [Slider(1, 10)]
    private int obstacleThresholdLevelUp = 3;

    [BoxGroup("Obstacles")]
    [SerializeField]
    [Slider(1, 10)]
    private int obstacleThresholdLevelDown = 2;

    [BoxGroup("Obstacles")]
    [SerializeField]
    private GameObject[] obstaclesAir;

    [BoxGroup("Obstacles")]
    [SerializeField]
    private GameObject[] obstaclesWater;

    private float timer;
    private bool spawnEnabled;

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
