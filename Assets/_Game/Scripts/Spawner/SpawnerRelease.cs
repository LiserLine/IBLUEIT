using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Spawner
{
    public float InspiratoryHeightLevel => 1f + insHeightAccumulator / 100f;
    public float ExpiratoryHeightLevel => 1f + expHeightAccumulator / 100f;
    public float ExpiratorySizeLevel => 1f + expSizeAccumulator / 100f;

    private float insHeightAccumulator;
    private float expHeightAccumulator;

    private float insSizeAccumulator;
    private float expSizeAccumulator;

    private bool isRelaxTime;
    private bool isRelaxTimeDone;

    public delegate void ObjectReleasedHandler(EnemyType type, ref GameObject obj1, ref GameObject obj2);
    public event ObjectReleasedHandler OnObjectReleased;

    public delegate void RelaxTimeStartHandler();
    public event RelaxTimeStartHandler OnRelaxTimeStart;

    private const float minDistanceBetweenSpawns = 3f;

    [Button("Spawn Objects")]
    private void Spawn()
    {
        if (isRelaxTime && !isRelaxTimeDone)
        {
            SpawnRelaxTime();
            isRelaxTime = false;
            isRelaxTimeDone = true;
        }
        else
        {
            switch (spawnObjects)
            {
                case EnemyType.Targets:
                    ReleaseTargets();
                    break;
                case EnemyType.Obstacles:
                    ReleaseObstacles();
                    break;
                default:
                    if (Random.Range(0, 2) == 0) ReleaseTargets();
                    else ReleaseObstacles();
                    break;
            }
        }
    }

    private void DistanciateSpawns(ref GameObject next)
    {
        if (objectsOnScene.Count < 1)
            return;

        var dist = minDistanceBetweenSpawns + (1f + (1f / (float)PlayerData.Player.Disfunction));

        var lastObj = objectsOnScene.Last();
        var lastPos = lastObj.transform.position.x + lastObj.transform.localScale.x / 2f;

        var relativeDistance = next.transform.position.x - lastPos;

        if (relativeDistance > 0 && relativeDistance < dist)
            next.transform.Translate(dist - relativeDistance, 0f, 0f);
        else if (relativeDistance < dist && relativeDistance < 0)
            next.transform.Translate(-relativeDistance + dist, 0f, 0f);
    }

    private void UpdateSpeed(ref GameObject go) => go.GetComponent<MoveObject>().speed = objectSpeed;

    #region Targets

    private void ReleaseTargets()
    {
        GameObject airObj, waterObj;

        InstanciateTargetAir(out airObj);
        InstanciateTargetWater(out waterObj);

        DistanciateSpawns(ref airObj);
        DistanciateTargets(ref airObj, ref waterObj);

        UpdateSpeed(ref airObj);
        UpdateSpeed(ref waterObj);

        objectsOnScene.Add(airObj);
        objectsOnScene.Add(waterObj);

        OnObjectReleased?.Invoke(EnemyType.Targets, ref airObj, ref waterObj);
    }

    private void DistanciateTargets(ref GameObject first, ref GameObject second)
    {
        second.transform.Translate(first.transform.position.x + 2f - second.transform.position.x, 0f, 0f);
    }

    private void InstanciateTargetAir(out GameObject spawned)
    {
        var index = Random.Range(0, targetsAir.Length);
        spawned = Instantiate(targetsAir[index],
            transform.position,
            transform.rotation,
            transform);

        var posY = (1f + insHeightAccumulator / 100f) * CameraLimits.Boundary *
                   Random.Range(gameDifficulties[0] / 100f, gameDifficulty / 100f);

        posY = Mathf.Clamp(posY, gameDifficulties[0] / 100f * CameraLimits.Boundary, CameraLimits.Boundary);

        spawned.transform.Translate(0f, posY, 0f);
    }

    private void InstanciateTargetWater(out GameObject spawned)
    {
        var index = Random.Range(0, targetsWater.Length);
        spawned = Instantiate(targetsWater[index],
            transform.position,
            transform.rotation,
            transform);

        var posY = (1f + expHeightAccumulator / 100f) * CameraLimits.Boundary *
                   Random.Range(gameDifficulties[0] / 100f, gameDifficulty / 100f);

        posY = Mathf.Clamp(-posY, -CameraLimits.Boundary, gameDifficulties[0] / 100f * -CameraLimits.Boundary);

        spawned.transform.Translate(0f, posY, 0f);
    }

    #endregion

    #region Obstacles

    private void ReleaseObstacles()
    {
        GameObject airObj, waterObj;

        InstanciateObstacleAir(out airObj);
        InstanciateObstacleWater(out waterObj);

        DistanciateSpawns(ref waterObj);
        DistanciateObstacles(ref waterObj, ref airObj);

        UpdateSpeed(ref airObj);
        UpdateSpeed(ref waterObj);

        objectsOnScene.Add(waterObj);
        objectsOnScene.Add(airObj);

        OnObjectReleased?.Invoke(EnemyType.Obstacles, ref waterObj, ref airObj);
    }

    private void DistanciateObstacles(ref GameObject first, ref GameObject second)
    {
        var firstPos = first.transform.position.x + first.transform.localScale.x / 2f;
        var secondPos = second.transform.position.x - second.transform.localScale.x / 2f;

        second.transform.Translate(firstPos + 2f - secondPos, 0f, 0f);
    }

    private void InstanciateObstacleAir(out GameObject spawned)
    {
        var index = Random.Range(0, obstaclesAir.Length);

        spawned = Instantiate(obstaclesAir[index],
            new Vector3(transform.position.x, 0f),
            transform.rotation,
            transform);

        var scaleFromPlayer = PlayerData.Player.RespiratoryInfo.ExpiratoryFlowTime / 1000f;

        var scale = scaleFromPlayer
                    * (1f + insSizeAccumulator / 100f)
                    * (float)PlayerData.Player.Disfunction
                    * gameDifficulty / 100f;

        scale = Mathf.Clamp(scale, scaleFromPlayer * 0.7f, scale);

        spawned.transform.localScale = new Vector3(scale, scale, 1);
        spawned.transform.Translate(0f, spawned.transform.localScale.y / 2, 0f);
    }

    private void InstanciateObstacleWater(out GameObject spawned)
    {
        var index = Random.Range(0, obstaclesWater.Length);

        spawned = Instantiate(obstaclesWater[index],
            new Vector3(transform.position.x, 0f),
            transform.rotation,
            transform);

        var scale = PlayerData.Player.RespiratoryInfo.ExpiratoryFlowTime / 1000f
                    * (1f + expSizeAccumulator / 100f);

        spawned.transform.localScale = new Vector3(scale, scale, 1);
        spawned.transform.Translate(0f, -spawned.transform.localScale.y / 2, 0f);
    }

    #endregion

    #region Relax Time

    [Button("Spawn Relax Time")]
    private void SpawnRelaxTime()
    {
        var disfunction = (int)PlayerData.Player.Disfunction;
        var objects = new GameObject[11 + 4 * disfunction];
        int i;


        var refPos = objectsOnScene.Last().transform.position;
        refPos.y = 0;

        for (i = 0; i < 4; i++)
        {
            objects[i] = Instantiate(relaxInsPrefab, refPos, transform.rotation, transform);

            if (i == 0)
            {
                DistanciateSpawns(ref objects[0]);
                refPos = objects[0].transform.position;
                refPos.y = 0;
            }

            ObjectsOnScene.Add(objects[i]);
        }

        for (; i < 11; i++)
            objects[i] = Instantiate(relaxZeroPrefab, refPos, transform.rotation, transform);

        for (; i < objects.Length; i++)
            objects[i] = Instantiate(relaxExpPrefab, refPos, transform.rotation, transform);

        for (i = 0; i < objects.Length; i++)
        {
            UpdateSpeed(ref objects[i]);
            objects[i].transform.Translate(i / 2f, 0f, 0f);
        }

        for (i = 0; i < objects.Length; i++)
        {
            if (i < 4)
                objects[i].transform.Translate(0f, 0.1f * CameraLimits.Boundary, 0f);
            else if (i > 10)
                objects[i].transform.Translate(0f, 0.15f * -CameraLimits.Boundary, 0f);
        }

        foreach (var go in objects)
            objectsOnScene.Add(go);

        OnRelaxTimeStart?.Invoke();
    }

    #endregion

}
