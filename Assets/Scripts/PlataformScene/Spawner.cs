using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Transform _transform;
    private float _delta;
    private float _spawnEveryXSec;
    private float _movementSpeed;

    [Header("Settings")]
    public GameObject[] Obstacles = new GameObject[1];
    public GameObject[] Items = new GameObject[1];

    private void Awake()
    {

#if UNITY_EDITOR
        if (GameManager.Instance.Player == null)
        {
            GameManager.Instance.Player = new Player
            {
                Name = "NetRunner",
                Id = 0
            };
        }
#endif

        _transform = this.GetComponent<Transform>();

        //ToDo - carrega dados do jogador (limites)
        //ToDo - setar maximos pro jogador

        _movementSpeed = GameManager.Instance.Player.RespiratoryInfo.RespirationFrequency * 0.3f; //ToDo - should we be using this?                
        _spawnEveryXSec = 5f;
    }

    private void Update()
    {
        UpdateYPosition();
        ReleaseObject();
    }

    private void UpdateYPosition()
    {
        _transform.position = new Vector3(_transform.position.x,
            Mathf.Sin(Time.time * _movementSpeed) * GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow,
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
            Instantiate(Items[0], _transform.position, _transform.rotation);
        }

        _delta = 0f;
    }
}