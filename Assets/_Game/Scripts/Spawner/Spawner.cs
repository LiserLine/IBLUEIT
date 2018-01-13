using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum EnemyType //ToDo - move from here
{
    Targets = 0,
    TargetsAndObstacles = 1,
    Obstacles = 2
}

public partial class Spawner : Singleton<Spawner>
{
    public EnemyType SpawnObjects => spawnObjects;
    public float SpawnDelay => spawnDelay;
    public float GameDifficulty => gameDifficulty;
    public int ObjectsOnScene => objectsOnScene.Count;
    public int RelaxTrigger => relaxBonusTrigger;

    /// <summary>
    /// Write an stage ID to load settings from a StageList before starting the game.
    /// </summary>
    public static int StageToLoad;

    private float timer;
    private float savedSpawnDelay;
    private bool spawnEnabled;
    private List<GameObject> objectsOnScene;

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

    protected override void Awake()
    {
        base.Awake();

        objectsOnScene?.Clear();
        objectsOnScene = new List<GameObject>();

        if (StageToLoad > 0)
            LoadCsv(StageToLoad);

        savedSpawnDelay = spawnDelay;

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        StageManager.Instance.OnStageStart += EnableSpawn;
        StageManager.Instance.OnStageTimeUp += DisableSpawn;
        Player.Instance.OnPlayerDeath += DisableSpawn;
        Player.Instance.OnEnemyHit += Player_OnEnemyHit;
        Scorer.Instance.OnEnemyMiss += Player_OnEnemyMiss;
        Destroyer.OnObjectDestroyed += RemoveObject;
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

        Clean();
    }

    private void Clean()
    {
        //ToDo - clean anim? like cuphead or w/e

        if (objectsOnScene.Count < 1)
            return;

        foreach (var go in objectsOnScene)
            Destroy(go);

        objectsOnScene.Clear();
    }

    private void RemoveObject(GameObject goner)
    {
        objectsOnScene.Remove(goner);
    }
}
