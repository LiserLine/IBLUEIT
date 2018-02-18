using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public enum SpawnObject
{
    Targets = 1,
    TargetsAndObstacles = 2,
    Obstacles = 3
}

public partial class Spawner : MonoBehaviour
{
    [BoxGroup("Obstacles")]
    [SerializeField]
    private GameObject[] obstaclesAir;

    [BoxGroup("Obstacles")]
    [SerializeField]
    private GameObject[] obstaclesWater;

    [BoxGroup("Relax Time")]
    [SerializeField]
    private GameObject relaxInsPrefab, relaxExpPrefab, relaxZeroPrefab;

    private bool spawnEnabled;
    [BoxGroup("Targets")]
    [SerializeField]
    private GameObject[] targetsAir;

    [BoxGroup("Targets")]
    [SerializeField]
    private GameObject[] targetsWater;

    private float timer;

    public int ObjectsOnScene => SpawnedObjects.Length;
    public Transform[] SpawnedObjects => this.gameObject.GetComponentsInChildren<Transform>().Where(o => !o.name.Equals("Spawner") && !o.name.Equals("Sprite")).ToArray();

    private void Awake()
    {
        var stgMgr = FindObjectOfType<StageManager>();
        stgMgr.OnStageStart += EnableSpawn;
        stgMgr.OnStageTimeUp += ReleaseRelaxTime;
        stgMgr.OnStageTimeUp += DisableSpawn;
        stgMgr.OnStageEnd += Clean;

        var plr = FindObjectOfType<Player>();
        plr.OnPlayerDeath += DisableSpawn;
        plr.OnEnemyHit += Player_OnEnemyHit;

        timer = Stage.Loaded.SpawnDelay;
    }

    private void Clean()
    {
        if (SpawnedObjects.Length < 1)
            return;

        foreach (var tform in SpawnedObjects)
        {
            if (tform == this.gameObject.transform)
                continue;

            Destroy(tform.gameObject);
        }
    }

    [Button("Disable Spawn")]
    private void DisableSpawn()
    {
        if (!spawnEnabled)
            return;

        spawnEnabled = false;
        timer = 0f;
    }

    [Button("Enable Spawn")]
    private void EnableSpawn()
    {
        if (spawnEnabled)
            return;

        spawnEnabled = true;
        timer = Stage.Loaded.SpawnDelay;
    }

    private void Update()
    {
        if (!spawnEnabled)
            return;

        timer += Time.deltaTime;

        if (timer > Stage.Loaded.SpawnDelay)
        {
            timer = 0f;
            Spawn();
        }
    }
}
