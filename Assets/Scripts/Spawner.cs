using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Transform _transform;
    private float _delta;
    private float _spawnEveryXSec;
    private float _movementSpeed;

    private Pacient _playerPacient;

    [Header("Settings")]
    public GameObject[] Obstacles = new GameObject[1];
    public GameObject[] Items = new GameObject[1];

    private void Start()
    {
        _transform = this.GetComponent<Transform>();

        //carrega dados do jogador (limites)
        //setar maximos pro jogador

        //debug
        _playerPacient = new Pacient();
        _playerPacient.Name = "Dummy";
        _playerPacient.Id = 1;
        _playerPacient.ExpiratoryPeakFlow = 5;
        _playerPacient.InspiratoryPeakFlow = -5;
        _playerPacient.RespirationFrequency = 10;
        _movementSpeed = _playerPacient.RespirationFrequency * 0.3f; //?? should we be using this?                
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
            Mathf.Sin(Time.time * _movementSpeed) * _playerPacient.ExpiratoryPeakFlow,
            _transform.position.z);
    }

    private void ReleaseObject()
    {
        _delta += Time.deltaTime;

        if (!(_delta > _spawnEveryXSec)) return;

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