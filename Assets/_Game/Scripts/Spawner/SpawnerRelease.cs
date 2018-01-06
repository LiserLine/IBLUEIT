using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public partial class Spawner
{
    private float insSizeAccumulator;
    private float insHeightAccumulator;
    private float expSizeAccumulator;
    private float expHeightAccumulator;

    public delegate void ObjectReleasedHandler(EnemyType enemyType, ref GameObject go1, ref GameObject go2);
    public static event ObjectReleasedHandler OnObjectReleased;

    private float cameraBounds = 2.5f; //ToDo - get this properlly

    private EnemyType lastEnemyType;

    [Button("Release Objects")]
    private void Release()
    {
        GameObject go1, go2;

        switch (this.spawnObjects)
        {
            case EnemyType.Targets:
                ReleaseTargetAir(out go1);
                ReleaseTargetWater(out go2);
                DistanciateTargets(ref go2, ref go1);
                lastEnemyType = EnemyType.Targets;
                break;
            case EnemyType.Obstacles:
                ReleaseObstacleAir(out go1);
                ReleaseObstacleWater(out go2);
                DistanciateObstacles(ref go2, ref go1);
                lastEnemyType = EnemyType.Obstacles;
                break;
            default: //EnemyType.TargetsAndObstacles
                if (Random.Range(0, 2) == 0)
                {
                    ReleaseTargetAir(out go1);
                    ReleaseTargetWater(out go2);
                    DistanciateTargets(ref go2, ref go1);
                    lastEnemyType = EnemyType.Targets;
                }
                else
                {
                    ReleaseObstacleAir(out go1);
                    ReleaseObstacleWater(out go2);
                    DistanciateObstacles(ref go2, ref go1);
                    lastEnemyType = EnemyType.Obstacles;
                }
                break;
        }

        go1.GetComponent<MoveObject>().speed = objectSpeed;
        go2.GetComponent<MoveObject>().speed = objectSpeed;

        OnObjectReleased?.Invoke(lastEnemyType, ref go1, ref go2);
    }

    #region Targets

    private void DistanciateTargets(ref GameObject go1, ref GameObject go2)
    {
        var go1pos = go1.transform.position;
        var go2pos = go2.transform.position;

        if (lastEnemyType == EnemyType.Obstacles)
            go1pos.x += distanceBetweenTargets * 1.5f;

        go2pos.x = go1pos.x + distanceBetweenTargets;

        go1.transform.position = go1pos;
        go2.transform.position = go2pos;
    }

    private void ReleaseTargetAir(out GameObject spawned)
    {
        var index = Random.Range(0, this.targetsAir.Length);
        spawned = Instantiate(targetsAir[index],
            this.transform.position,
            this.transform.rotation,
            this.transform);

        var posY = (1f + this.insHeightAccumulator / 100f)
                   * Player.playerDto.RespiratoryInfo.InspiratoryPeakFlow
                   * cameraBounds / Mathf.Abs(Player.playerDto.RespiratoryInfo.InspiratoryPeakFlow)
                   * Random.Range(0.4f, this.gameDifficulty / 100f);

        var pos = spawned.transform.position;
        pos.y = posY;
        spawned.transform.position = pos;
    }

    private void ReleaseTargetWater(out GameObject spawned)
    {
        var index = Random.Range(0, this.targetsWater.Length);
        spawned = Instantiate(targetsWater[index],
            this.transform.position,
            this.transform.rotation,
            this.transform);

        var posY = (1f + this.expHeightAccumulator / 100f)
                   * Player.playerDto.RespiratoryInfo.ExpiratoryPeakFlow
                   * cameraBounds / Mathf.Abs(Player.playerDto.RespiratoryInfo.ExpiratoryPeakFlow)
                   * Random.Range(0.4f, this.gameDifficulty / 100f);

        var pos = spawned.transform.position;
        pos.y = posY;
        spawned.transform.position = pos;
    }

    #endregion

    #region Obstacles

    private void DistanciateObstacles(ref GameObject go1, ref GameObject go2)
    {
        var go1pos = go1.transform.position;
        var go2pos = go2.transform.position;

        if (lastEnemyType == EnemyType.Obstacles)
            go1pos.x += distanceBetweenObstacles;

        go2pos.x = go1pos.x + distanceBetweenObstacles;

        go1.transform.position = go1pos;
        go2.transform.position = go2pos;
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

        var tmpPosition = spawned.transform.position;
        tmpPosition.y += spawned.transform.localScale.y / 2;
        spawned.transform.position = tmpPosition;
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
        //* (float)Player.playerDto.Disfunction
        //* this.gameDifficulty / 100f;

        spawned.transform.localScale = new Vector3(scale, scale, 1);

        var tmpPosition = spawned.transform.position;
        tmpPosition.y -= spawned.transform.localScale.y / 2;
        spawned.transform.position = tmpPosition;
    }

    #endregion

}
