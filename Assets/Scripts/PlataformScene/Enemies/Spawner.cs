using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float _dt;
    private float _spawnEveryXSec;
    private float _plrBasedHeight;

    private float _levelInfluence = 1; // ToDo - based on CSV
    private float _disfunctionInfluence = 1; // ToDo - based on Enum

    public GameObject[] Obstacles;
    public GameObject[] Targets;

    private void OnEnable()
    {
        _plrBasedHeight = GameManager.Instance.Player.RespiratoryInfo.RespirationFrequency * _levelInfluence;
        _spawnEveryXSec = GameManager.Instance.Player.RespiratoryInfo.RespirationFrequency;
    }

    private void Update()
    {
        if (!SerialGetOffset.IsUsingOffset)
            return;

        UpdatePosition();

        _dt += Time.deltaTime;
        if (_dt >= _spawnEveryXSec)
        {
            ReleaseObject();
            _dt = 0f;
        }
    }

    private void UpdatePosition()
    {
        var sineOnTime = Mathf.Sin(Time.time);
        var cameraHeightFromZero = 9; // ToDo - get this properlly

        var nextPos = new Vector3
        {
            x = this.transform.position.x,
            y = sineOnTime * cameraHeightFromZero / (sineOnTime >= 0f ? GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow : GameManager.Instance.Player.RespiratoryInfo.InspiratoryPeakFlow) ,
            z = this.transform.position.z
        };

        this.transform.position = nextPos;
    }

    private void ReleaseObject()
    {
        var rnd = Random.Range(0, 2);
        if (rnd == 0) //Obstacles
        {
            //Instantiate(Obstacles[0], this.transform); <- Waveform movement
            var size = Random.Range(1, 4);
            Obstacles[0].transform.localScale = new Vector3(size, size * 3, 1f);
            Instantiate(Obstacles[0], new Vector3(this.transform.position.x, 0f), this.transform.rotation);
        }
        else //Items
        {
            Instantiate(Targets[0], this.transform.position, this.transform.rotation);
        }
    }
}