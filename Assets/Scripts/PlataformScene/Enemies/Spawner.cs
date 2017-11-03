using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float _dt;
    private float _spawnEveryXSec;

    private float _distanceBetweenSpawns;

    public GameObject[] Obstacles;
    public GameObject[] Targets;

    private void OnEnable()
    {
        var stage = (PlataformStage)GameManager.Instance.Stage;
        _spawnEveryXSec = (float)stage.TimeLimit / stage.SpawnQuantitity;
        stage.OnStageEnd += DestroySpawnedObjects;
        _distanceBetweenSpawns = GameManager.Instance.Player.Disfunction == Disfunctions.Restrictive ? 7f : 14f;
        _dt = _spawnEveryXSec;
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
        var cameraBounds = 9; // ToDo - get this properlly

        var nextPos = new Vector3
        {
            x = this.transform.position.x,
            y = sineOnTime * cameraBounds / GameManager.Instance.Stage.Id,
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

        //var randomHeightOffset = Random.Range(0, 2) * 2 - 1; //ToDo - this is strange during gameplay

        var disfunctionMultiplier = (float)GameManager.Instance.Player.Disfunction;

        //var go = Instantiate(Obstacles[obstacleIndex1], new Vector3(this.transform.position.x, -randomHeightOffset), this.transform.rotation);
        //var go2 = Instantiate(Obstacles[obstacleIndex2], new Vector3(this.transform.position.x + _distanceBetweenSpawns, randomHeightOffset), this.transform.rotation);

        var go = Instantiate(Obstacles[obstacleIndex1], new Vector3(this.transform.position.x, 1f + disfunctionMultiplier / 3), this.transform.rotation);
        var go2 = Instantiate(Obstacles[obstacleIndex2], new Vector3(this.transform.position.x + _distanceBetweenSpawns, -1f - disfunctionMultiplier * 2 / 2), this.transform.rotation);

        go.transform.localScale = new Vector3(go.transform.localScale.x * sizeMultiplier, go.transform.localScale.y * sizeMultiplier, 1);
        go2.transform.localScale = new Vector3(go2.transform.localScale.x * sizeMultiplier * disfunctionMultiplier, go2.transform.localScale.y * sizeMultiplier * disfunctionMultiplier, 1);
    }

    private void DestroySpawnedObjects()
    {
        var spawnedObjs = GameObject.FindGameObjectsWithTag("SpawnedObject");

        foreach (var obj in spawnedObjs)
            Destroy(obj);
    }
}