using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public enum EnemyType //ToDo - move from here
{
    Targets = 0,
    TargetsAndObstacles = 1,
    Obstacles = 2
}

public partial class Spawner : MonoBehaviour
{
    public EnemyType SpawnObjects => spawnObjects;
    public float SpawnDelay => spawnDelay;
    public float GameDifficulty => gameDifficulty;
    public static int ObjectsRemaining => spawnedList.Count;

    /// <summary>
    /// Write an stage ID to load settings from a StageList before starting the game.
    /// </summary>
    public static int StageToLoad;

    private float timer;
    private float savedSpawnDelay;
    private bool spawnEnabled;

    private static List<GameObject> spawnedList;

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
    private float gameDifficulty = 50f;

    private readonly float[] gameDifficulties = { 30f, 40f, 50f, 60f, 70f, 80f, 90f, 100f };

    [BoxGroup("Stage Settings")]
    [Slider(1f, 5f)]
    [SerializeField]
    private float objectSpeed = 1;

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

    [BoxGroup("Relax Time")]
    [Tooltip("Number of bonus coins required to start bonus time.")]
    [SerializeField]
    [Slider(1, 5)]
    private int relaxBonusTrigger = 2;

    [BoxGroup("Relax Time")]
    [SerializeField]
    private GameObject relaxInsPrefab, relaxExpPrefab, relaxZeroPrefab;

    [SerializeField]
    private StageManager stageManager;

    private void Awake()
    {
        spawnedList?.Clear();
        spawnedList = new List<GameObject>();

        if (StageToLoad > 0)
            LoadCsv(StageToLoad);

        savedSpawnDelay = spawnDelay;
    }

    private void OnEnable()
    {
        StageManager.OnStageStart += EnableSpawn;
        StageManager.OnStageTimeUp += DisableSpawn;
        Player.OnPlayerDeath += DisableSpawn;
        Player.OnEnemyHit += Player_OnEnemyHit;
        Scorer.OnEnemyMiss += Player_OnEnemyMiss;
        Destroyer.OnObjectDestroyed += RemoveObject;
    }

    private void OnDisable()
    {
        StageManager.OnStageStart -= EnableSpawn;
        StageManager.OnStageTimeUp -= DisableSpawn;
        Player.OnPlayerDeath -= DisableSpawn;
        Player.OnEnemyHit -= Player_OnEnemyHit;
        Scorer.OnEnemyMiss -= Player_OnEnemyMiss;
    }

    private void Update()
    {
        if (!spawnEnabled)
            return;

        timer += Time.deltaTime;

        if (timer > spawnDelay)
        {
            timer = 0f;
            Spawn();
        }
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

    //private void RemoveObject(int guid)
    //{
    //    var first = spawnedList.First(x => x.GetHashCode() == guid);
    //    spawnedList.Remove(first);
    //}

    private void RemoveObject(GameObject goner)
    {
        spawnedList.Remove(goner);
    }
}
