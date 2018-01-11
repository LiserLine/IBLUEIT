using NaughtyAttributes;
using UnityEngine;

public partial class Spawner
{
    private float insSizeAccumulator;
    private float insHeightAccumulator;
    private float expSizeAccumulator;
    private float expHeightAccumulator;
    private bool isRelaxTime;
    private bool isRelaxTimeDone;

    public delegate void ObjectReleasedHandler(EnemyType type, ref GameObject obj1, ref GameObject obj2);
    public static event ObjectReleasedHandler OnObjectReleased;

    public delegate void RelaxTimeStartHandler();
    public static event RelaxTimeStartHandler OnRelaxTimeStart;

    private EnemyType lastEnemyType;

    [Button("Release Objects")]
    private void Release()
    {
        if (isRelaxTime && !isRelaxTimeDone)
        {
            spawnDelay = 20f;
            ReleaseRelaxTime();
            isRelaxTime = false;
            isRelaxTimeDone = true;
        }
        else
        {
            spawnDelay = _spawnDelay;
            switch (this.spawnObjects)
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

    #region Targets

    private void ReleaseTargets()
    {
        GameObject airObj, waterObj;

        ReleaseTargetAir(out airObj);
        ReleaseTargetWater(out waterObj);
        DistanciateTargets(ref airObj, ref waterObj);
        lastEnemyType = EnemyType.Targets;

        airObj.GetComponent<MoveObject>().speed = objectSpeed;
        waterObj.GetComponent<MoveObject>().speed = objectSpeed;

        OnObjectReleased?.Invoke(lastEnemyType, ref airObj, ref waterObj);
    }

    private void DistanciateTargets(ref GameObject go1, ref GameObject go2)
    {
        if (lastEnemyType == EnemyType.Obstacles)
            go1.transform.Translate(distanceBetweenTargets * 1.5f, 0f, 0f);

        go2.transform.Translate(go1.transform.position.x + distanceBetweenTargets - go2.transform.position.x, 0f, 0f);
    }

    private void ReleaseTargetAir(out GameObject spawned)
    {
        var index = Random.Range(0, this.targetsAir.Length);
        spawned = Instantiate(targetsAir[index],
            this.transform.position,
            this.transform.rotation,
            this.transform);

        var posY = (1f + this.insHeightAccumulator / 100f) * CameraLimits.Boundary *
                   Random.Range((gameDifficulties[0] / 100f), this.gameDifficulty / 100f);

        posY = Utils.Clip(posY, (gameDifficulties[0] / 100f) * CameraLimits.Boundary, CameraLimits.Boundary);

        spawned.transform.Translate(0f, posY, 0f);
    }

    private void ReleaseTargetWater(out GameObject spawned)
    {
        var index = Random.Range(0, this.targetsWater.Length);
        spawned = Instantiate(targetsWater[index],
            this.transform.position,
            this.transform.rotation,
            this.transform);

        var posY = (1f + this.expHeightAccumulator / 100f) * CameraLimits.Boundary *
                   Random.Range((gameDifficulties[0] / 100f), this.gameDifficulty / 100f);

        posY = Utils.Clip(-posY, -CameraLimits.Boundary, (gameDifficulties[0] / 100f) * -CameraLimits.Boundary);

        spawned.transform.Translate(0f, posY, 0f);
    }

    #endregion

    #region Obstacles

    private void ReleaseObstacles()
    {
        GameObject airObj, waterObj;

        ReleaseObstacleAir(out airObj);
        ReleaseObstacleWater(out waterObj);
        DistanciateObstacles(ref waterObj, ref airObj);
        lastEnemyType = EnemyType.Obstacles;

        airObj.GetComponent<MoveObject>().speed = objectSpeed;
        waterObj.GetComponent<MoveObject>().speed = objectSpeed;

        OnObjectReleased?.Invoke(lastEnemyType, ref airObj, ref waterObj);
    }

    private void DistanciateObstacles(ref GameObject go1, ref GameObject go2)
    {
        if (lastEnemyType == EnemyType.Obstacles)
            go1.transform.Translate(distanceBetweenObstacles, 0f, 0f);

        go2.transform.Translate(go1.transform.position.x + distanceBetweenObstacles - go2.transform.position.x, 0f, 0f);
    }

    private void ReleaseObstacleAir(out GameObject spawned)
    {
        var index = Random.Range(0, obstaclesAir.Length);

        spawned = Instantiate(obstaclesAir[index],
            new Vector3(this.transform.position.x, 0f),
            this.transform.rotation,
            this.transform);

        var scale = (Player.Data.RespiratoryInfo.ExpiratoryFlowTime / 1000f)
                    * (1f + (this.insSizeAccumulator / 100f))
                    * (float)Player.Data.Disfunction
                    * this.gameDifficulty / 100f;

        spawned.transform.localScale = new Vector3(scale, scale, 1);
        spawned.transform.Translate(0f, spawned.transform.localScale.y / 2, 0f);
    }

    private void ReleaseObstacleWater(out GameObject spawned)
    {
        var index = Random.Range(0, obstaclesWater.Length);

        spawned = Instantiate(obstaclesWater[index],
            new Vector3(this.transform.position.x, 0f),
            this.transform.rotation,
            this.transform);

        var scale = (Player.Data.RespiratoryInfo.ExpiratoryFlowTime / 1000f)
                    * (1f + (this.expSizeAccumulator / 100f));

        spawned.transform.localScale = new Vector3(scale, scale, 1);
        spawned.transform.Translate(0f, -spawned.transform.localScale.y / 2, 0f);
    }

    #endregion

    #region Relax Time

    [Button("Release Relax Time")]
    private void ReleaseRelaxTime()
    {
        var disfunction = (int)Player.Data.Disfunction;
        var objects = new GameObject[11 + 4 * disfunction];
        int i;

        for (i = 0; i < 4; i++)
            objects[i] = Instantiate(relaxInsPrefab, this.transform.position, this.transform.rotation, this.transform);

        for (; i < 11; i++)
            objects[i] = Instantiate(relaxZeroPrefab, this.transform.position, this.transform.rotation, this.transform);

        for (; i < objects.Length; i++)
            objects[i] = Instantiate(relaxExpPrefab, this.transform.position, this.transform.rotation, this.transform);

        for (i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MoveObject>().speed = objectSpeed;
            objects[i].transform.Translate(i / 2f, 0f, 0f);
        }

        for (i = 0; i < objects.Length; i++)
        {
            if (i < 4)
                objects[i].transform.Translate(0f, 0.1f * CameraLimits.Boundary, 0f);
            else if (i > 10)
                objects[i].transform.Translate(0f, 0.15f * -CameraLimits.Boundary, 0f);
        }

        OnRelaxTimeStart?.Invoke();
    }

    #endregion

}
