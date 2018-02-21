﻿using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Spawner
{
    private const float minDistanceBetweenSpawns = 3f;
    private float expHeightAcc = 1f;
    private float expSizeAcc = 1f;
    private float insHeightAcc = 1f;
    private float insSizeAcc = 1f;
    private bool spawnRelaxTime;

    public delegate void ObjectReleasedHandler(SpawnObject type, ref GameObject obj1, ref GameObject obj2);

    public event ObjectReleasedHandler OnObjectReleased;

    public bool RelaxTimeSpawned { get; private set; }

    private void DistanciateSpawns(ref GameObject next)
    {
        var dist = minDistanceBetweenSpawns + (1f + (1f / (float)Pacient.Loaded.Condition));

        var lastObj = SpawnedObjects.Length > 2 ? SpawnedObjects[SpawnedObjects.Length - 3] : this.transform;
        var lastPos = lastObj.position.x + lastObj.localScale.x / 2f;

        var relativeDistance = next.transform.position.x - lastPos;

        if (relativeDistance > 0 && relativeDistance < dist)
            next.transform.Translate(dist - relativeDistance, 0f, 0f);
        else if (relativeDistance < dist && relativeDistance < 0)
            next.transform.Translate(-relativeDistance + dist, 0f, 0f);
    }

    [Button("Release")]
    private void Spawn()
    {
        switch (Stage.Loaded.SpawnObject)
        {
            case SpawnObject.Targets:
                ReleaseTargets();
                break;

            case SpawnObject.Obstacles:
                ReleaseObstacles();
                break;

            default:
                if (Random.Range(0, 2) == 0)
                    ReleaseTargets();
                else
                    ReleaseObstacles();
                break;
        }
    }

    private void UpdateSpeed(ref GameObject go) => go.GetComponent<MoveObject>().Speed = Stage.Loaded.ObjectSpeedMultiplier;

    #region Targets

    private void DistanciateTargets(ref GameObject first, ref GameObject second) =>
        second.transform.Translate(first.transform.position.x +
            Mathf.Clamp(Pacient.Loaded.Capacities.RespCycleDuration / 2500f, 1f, Pacient.Loaded.Capacities.RespCycleDuration / 2500f) -
            second.transform.position.x, 0f, 0f);

    private void InstanciateTargetAir(out GameObject spawned)
    {
        var index = Random.Range(0, targetsAir.Length);
        spawned = Instantiate(targetsAir[index],
            transform.position,
            transform.rotation,
            transform);

        var posY = insHeightAcc * CameraLimits.Boundary * Random.Range(0.2f, Stage.Loaded.GameDifficulty);

        posY = Mathf.Clamp(posY, 0.2f * CameraLimits.Boundary, CameraLimits.Boundary);

        spawned.transform.Translate(0f, posY, 0f);
    }

    private void InstanciateTargetWater(out GameObject spawned)
    {
        var index = Random.Range(0, targetsWater.Length);
        spawned = Instantiate(targetsWater[index],
            transform.position,
            transform.rotation,
            transform);

        var posY = expHeightAcc * CameraLimits.Boundary * Random.Range(0.2f, Stage.Loaded.GameDifficulty);

        posY = Mathf.Clamp(-posY, -CameraLimits.Boundary, 0.2f * -CameraLimits.Boundary);

        spawned.transform.Translate(0f, posY, 0f);
    }

    private void ReleaseTargets()
    {
        GameObject airObj, waterObj;

        InstanciateTargetAir(out airObj);
        InstanciateTargetWater(out waterObj);

        DistanciateSpawns(ref airObj);
        DistanciateTargets(ref airObj, ref waterObj);

        UpdateSpeed(ref airObj);
        UpdateSpeed(ref waterObj);

        OnObjectReleased?.Invoke(SpawnObject.Targets, ref airObj, ref waterObj);
    }

    #endregion Targets

    #region Obstacles

    private void DistanciateObstacles(ref GameObject first, ref GameObject second)
    {
        var firstPos = first.transform.position.x + first.transform.localScale.x / 2f;
        var secondPos = second.transform.position.x - second.transform.localScale.x / 2f;

        var limit = Pacient.Loaded.Capacities.RespCycleDuration / 3000f;

        second.transform.Translate(firstPos + Mathf.Clamp(limit, 2.5f, limit) - secondPos, 0f, 0f);
    }

    private void InstanciateObstacleAir(out GameObject spawned)
    {
        var index = Random.Range(0, obstaclesAir.Length);

        spawned = Instantiate(obstaclesAir[index],
            new Vector3(transform.position.x, 0f),
            transform.rotation,
            transform);

        var scale = Pacient.Loaded.Capacities.ExpFlowDuration / 1000f * expSizeAcc * (float)Pacient.Loaded.Condition * Stage.Loaded.GameDifficulty;

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

        var scale = Pacient.Loaded.Capacities.InsFlowDuration / 1000f * insSizeAcc;

        spawned.transform.localScale = new Vector3(scale, scale, 1);
        spawned.transform.Translate(0f, -spawned.transform.localScale.y / 2, 0f);
    }

    private void ReleaseObstacles()
    {
        GameObject airObj, waterObj;

        InstanciateObstacleWater(out waterObj);
        InstanciateObstacleAir(out airObj);

        DistanciateSpawns(ref waterObj);
        DistanciateObstacles(ref waterObj, ref airObj);

        UpdateSpeed(ref airObj);
        UpdateSpeed(ref waterObj);

        OnObjectReleased?.Invoke(SpawnObject.Obstacles, ref waterObj, ref airObj);
    }

    #endregion Obstacles

    #region Relax Time

    private void ReleaseRelaxTime()
    {
        if (!spawnRelaxTime || RelaxTimeSpawned)
            return;

        var disfunction = (int)Pacient.Loaded.Condition;
        var objects = new GameObject[11 + 4 * disfunction];
        int i;

        var refPos = SpawnedObjects.Last().position;
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
        }

        for (; i < 11; i++)
            objects[i] = Instantiate(relaxZeroPrefab, refPos, transform.rotation, transform);

        for (; i < objects.Length; i++)
            objects[i] = Instantiate(relaxExpPrefab, refPos, transform.rotation, transform);

        for (i = 0; i < objects.Length; i++)
        {
            UpdateSpeed(ref objects[i]);
            objects[i].transform.Translate(i / 1.75f, 0f, 0f);
        }

        for (i = 0; i < objects.Length; i++)
        {
            if (i < 4)
                objects[i].transform.Translate(0f, 0.1f * CameraLimits.Boundary, 0f);
            else if (i > 10)
                objects[i].transform.Translate(0f, 0.15f * -CameraLimits.Boundary, 0f);
        }

        RelaxTimeSpawned = true;
    }

    #endregion Relax Time

}