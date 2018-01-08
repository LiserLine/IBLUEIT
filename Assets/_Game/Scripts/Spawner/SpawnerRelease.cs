using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public partial class Spawner
{
    private float insSizeAccumulator;
    private float insHeightAccumulator;
    private float expSizeAccumulator;
    private float expHeightAccumulator;

    public delegate void ObjectReleasedHandler(EnemyType type, ref GameObject obj1, ref GameObject obj2);
    public static event ObjectReleasedHandler OnObjectReleased;
    
    private EnemyType lastEnemyType;

    [Button("Release Objects")]
    private void Release()
    {
        GameObject airObj, waterObj;

        switch (this.spawnObjects)
        {
            switch (this.spawnObjects)
            {
                case EnemyType.Targets:
                    ReleaseTargets();
                    break;
                case EnemyType.Obstacles:
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

        var posY = (1f + this.insHeightAccumulator / 100f) * CameraBoundary.Limit *
                   Random.Range(0.4f, this.gameDifficulty / 100f);

        spawned.transform.Translate(0f, posY, 0f);
    }

    private void ReleaseTargetWater(out GameObject spawned)
    {
        var index = Random.Range(0, this.targetsWater.Length);
        spawned = Instantiate(targetsWater[index],
            this.transform.position,
            this.transform.rotation,
            this.transform);

        var posY = (1f + this.expHeightAccumulator / 100f) * CameraBoundary.Limit *
                   Random.Range(0.4f, this.gameDifficulty / 100f);

        spawned.transform.Translate(0f, -posY, 0f);
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
        if(lastEnemyType == EnemyType.Obstacles)
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

        var scale = (Player.playerDto.RespiratoryInfo.ExpiratoryFlowTime / 1000f)
                    * (1f + (this.insSizeAccumulator / 100f))
                    * (float)Player.playerDto.Disfunction
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

        var scale = (Player.playerDto.RespiratoryInfo.ExpiratoryFlowTime / 1000f)
                    * (1f + (this.expSizeAccumulator / 100f));

        spawned.transform.localScale = new Vector3(scale, scale, 1);
        spawned.transform.Translate(0f, -spawned.transform.localScale.y / 2, 0f);
    }

    #endregion

    #region Relaxing Time

    private void RelaxingTime()
    {
        // soltar enemies de acordo com a doença da pessoa
    }

    #endregion

}
