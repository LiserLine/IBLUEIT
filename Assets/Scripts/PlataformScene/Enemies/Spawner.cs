using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float _dt;
    private float _spawnEveryXSec;
    private float _cameraBounds;

    private float _distanceBetweenSpawns;

    public GameObject[] Obstacles;
    public GameObject[] Targets;

    private void OnEnable()
    {
        var stage = (PlataformStage)GameManager.Instance.Stage;
        _spawnEveryXSec = (float)stage.TimeLimit / stage.SpawnQuantitity;
        stage.OnStageEnd += DestroySpawnedObjects;
        _distanceBetweenSpawns = 10f; // ToDO - is this good enough?
        _dt = _spawnEveryXSec;
        _cameraBounds = 9; // ToDo - get this properlly
    }

    private void OnDisable()
    {
        GameManager.Instance.Stage.OnStageEnd -= DestroySpawnedObjects;
    }

    private void Update()
    {
        if (!SerialGetOffset.IsUsingOffset)
            return;

        UpdatePosition();

        _dt += Time.deltaTime;
        if (_dt <= _spawnEveryXSec)
            return;

        ReleaseObject();
        _dt = 0f;
    }

    private void UpdatePosition()
    {
        var sineOnTime = Mathf.Sin(Time.time);

        var nextPos = new Vector3
        {
            x = this.transform.position.x,
            y = sineOnTime * _cameraBounds / GameManager.Instance.Stage.Id,
            z = this.transform.position.z
        };

        this.transform.position = nextPos;
    }

    private void ReleaseObject()
    {
        var stage = (PlataformStage)GameManager.Instance.Stage;

        if (stage.Elements == PlataformElements.Targets)
        {
            SpawnTarget(stage.TargetHeightMultiplier);
        }
        else if (stage.Elements == PlataformElements.Obstacles)
        {
            SpawnObstacle(stage.ObstacleSizeMultiplier);
        }
        else
        {
            var rnd = Random.Range(0, 2);
            if (rnd == (int)PlataformElements.Targets)
            {
                SpawnTarget(stage.TargetHeightMultiplier);
            }
            else
            {
                SpawnObstacle(stage.ObstacleSizeMultiplier);
            }
        }
    }

    private void SpawnTarget(float heightMultiplier)
    {
        var targetIndex = Random.Range(0, Targets.Length);
        var target = Instantiate(Targets[targetIndex], this.transform.position, this.transform.rotation);
        var nextPosition = target.transform.position;
        nextPosition.y *= heightMultiplier;
        target.transform.position = nextPosition;
    }

    private void SpawnObstacle(float sizeMultiplier)
    {
        var obstacleIndex1 = Random.Range(0, Obstacles.Length); // TODO properlly - alguns itens podem ser submersos, outros n
        var obstacleIndex2 = Random.Range(0, Obstacles.Length); // TODO properlly

        //ToDO - Remove if not used anymore
        //var disfunctionMultiplier = (float)GameManager.Instance.Player.Disfunction;

        //random //ToDO - Remove if not used anymore
        //var randomHeightOffset = Random.Range(0, 2) * 2 - 1; //ToDo - this is strange during gameplay
        //var go = Instantiate(Obstacles[obstacleIndex1], new Vector3(this.transform.position.x, -randomHeightOffset), this.transform.rotation);
        //var go2 = Instantiate(Obstacles[obstacleIndex2], new Vector3(this.transform.position.x + _distanceBetweenSpawns, randomHeightOffset), this.transform.rotation);

        var go = Instantiate(Obstacles[obstacleIndex1], new Vector3(this.transform.position.x, 0f), this.transform.rotation);
        var go2 = Instantiate(Obstacles[obstacleIndex2], new Vector3(this.transform.position.x + _distanceBetweenSpawns, 0f), this.transform.rotation);

        //ToDO - Remove if not used anymore
        //go.transform.localScale = new Vector3(go.transform.localScale.x * sizeMultiplier, go.transform.localScale.y * sizeMultiplier, 1);
        //go2.transform.localScale = new Vector3(go2.transform.localScale.x * sizeMultiplier * disfunctionMultiplier, go2.transform.localScale.y * sizeMultiplier * disfunctionMultiplier, 1);

        //ToDo - scales must be based on ins/exp on random too (if necessary)
        var scaleIns = GameManager.Instance.Player.RespiratoryInfo.InspiratoryFlowTime / 1000;
        var scaleExp = GameManager.Instance.Player.RespiratoryInfo.ExpiratoryFlowTime / 1000;

        //ToDo - Do I really need this?
        scaleExp *= GameConstants.UserPowerMercy + 1;
        scaleIns *= GameConstants.UserPowerMercy + 1;

        go.transform.localScale = new Vector3(scaleIns, scaleIns, 1);
        go2.transform.localScale = new Vector3(scaleExp, scaleExp, 1);

        var goPos = go.transform.position;
        var go2Pos = go2.transform.position;

        goPos.y -= go.transform.localScale.y / 2;
        go2Pos.y += go2.transform.localScale.y / 2;

        go.transform.position = goPos;
        go2.transform.position = go2Pos;
    }

    private void DestroySpawnedObjects()
    {
        var spawnedObjs = GameObject.FindGameObjectsWithTag("SpawnedObject");

        foreach (var obj in spawnedObjs)
            Destroy(obj);
    }
}