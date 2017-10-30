using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Transform _transform;
    private float _delta;
    private float _spawnEveryXSec;
    private float _movementSpeed;

    private float _levelInfluence = 1; // ToDo - based on CSV
    private float _disfuncionInfluencer = 1; // ToDo - based on Enum

    [Header("Settings")]
    public GameObject[] Obstacles = new GameObject[1];
    public GameObject[] Targets = new GameObject[1];

    private void OnEnable()
    {
        _transform = GetComponent<Transform>();
        _movementSpeed = GameManager.Instance.Player.RespiratoryInfo.RespirationFrequency * _levelInfluence;
        _spawnEveryXSec = GameManager.Instance.Player.RespiratoryInfo.RespirationFrequency;
    }

    private void Update()
    {
        if (!SerialGetOffset.IsUsingOffset)
            return;

        SetYPosision();
        ReleaseObject();
    }

    private void SetYPosision()
    {
        _transform.position = new Vector3(_transform.position.x,
            Mathf.Sin(Time.deltaTime * _movementSpeed) * GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow,
            _transform.position.z);
    }

    private void ReleaseObject()
    {
        _delta += Time.deltaTime;

        if (_delta < _spawnEveryXSec) return;

        var rnd = Random.Range(0, 2);
        if (rnd == 0) //Obstacles
        {
            //Instantiate(Obstacles[0], _transform); <- Waveform movement
            var size = Random.Range(1, 4);
            Obstacles[0].transform.localScale = new Vector3(size, size * 3, 1f);
            Instantiate(Obstacles[0], new Vector3(_transform.position.x, 0f), _transform.rotation);
        }
        else //Items
        {
            Instantiate(Targets[0], _transform.position, _transform.rotation);
        }

        _delta = 0f;
    }
}