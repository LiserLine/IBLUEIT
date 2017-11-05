using UnityEngine;

public class PositionOnSerial : MonoBehaviour
{
    private float _cameraBounds;
    private SerialController _serialController;
    private GameSessionRecorder _gameSessionRecorder;
    private PitacoRecorder _pitacoRecorder;
    private float _dt;

    public ControlBehaviour Behaviour;

    private void OnEnable()
    {
        _cameraBounds = 9; //ToDo - Get this properlly

        _serialController = GameObject.FindGameObjectWithTag("SerialController").GetComponent<SerialController>();
        _serialController.OnSerialMessageReceived += OnSerialMessageReceived;

        _gameSessionRecorder = new GameSessionRecorder("GameSession");
        _pitacoRecorder = new PitacoRecorder("Pitaco");

        _gameSessionRecorder.StartRecording();
        _pitacoRecorder.StartRecording();
    }

    private void OnDisable()
    {
        _serialController.OnSerialMessageReceived -= OnSerialMessageReceived;

        GameManager.Instance.Player.SessionsDone++;

        _pitacoRecorder.StopRecording();
        _pitacoRecorder.WriteData(GameManager.Instance.Player, GameManager.Instance.Stage, true);

        _gameSessionRecorder.StopRecording();
        _gameSessionRecorder.WriteData(GameManager.Instance.Player, GameManager.Instance.Stage, true);
    }

    private void Update()
    {
        //ChangeBehaviourHotkey();

        _dt += Time.deltaTime;
        if (_dt <= 1f / 30f)
            return;

        RecordObjects(); // 30FPS
        _dt = 0f;
    }

    private void RecordObjects()
    {
        var plr = GameObject.FindGameObjectWithTag("Player");
        var targets = GameObject.FindGameObjectsWithTag("SpawnedTarget");
        var obstacles = GameObject.FindGameObjectsWithTag("SpawnedObstacle");

        _gameSessionRecorder.RecordValue(Time.time, plr.tag, 0, plr.transform.position);

        foreach (var target in targets)
            _gameSessionRecorder.RecordValue(Time.time, target.tag, target.GetInstanceID(), target.transform.position);

        foreach (var obstacle in obstacles)
            _gameSessionRecorder.RecordValue(Time.time, obstacle.tag, obstacle.GetInstanceID(), obstacle.transform.position);
    }

    private void ChangeBehaviourHotkey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (Behaviour)
            {
                case ControlBehaviour.Absolute:
                    Behaviour = ControlBehaviour.Relative;
                    break;
                case ControlBehaviour.Relative:
                    Behaviour = ControlBehaviour.Absolute;
                    break;
            }

            Debug.LogFormat("Behaviour: {0}", Behaviour);
        }
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!SerialGetOffset.IsUsingOffset || msg.Length < 1)
            return;

        var sensorValue = GameUtilities.ParseFloat(msg) - SerialGetOffset.Offset;
        sensorValue = sensorValue < -GameConstants.PitacoThreshold || sensorValue > GameConstants.PitacoThreshold ? sensorValue : 0f;
        var limit = sensorValue > 0 ? GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow : -GameManager.Instance.Player.RespiratoryInfo.InspiratoryPeakFlow;

        var nextPosition = sensorValue * _cameraBounds / limit;
        if (nextPosition > _cameraBounds)
        {
            nextPosition = _cameraBounds + 1;
        }
        else if (nextPosition < -_cameraBounds)
        {
            nextPosition = -_cameraBounds - 1;
        }

        // se for pra baixo, descer mais dependendo da força do paciente

        Vector3 a = this.transform.position;
        Vector3 b = Vector3.zero;
        switch (Behaviour)
        {
            case ControlBehaviour.Absolute:
                b = new Vector3(this.transform.position.x, -nextPosition, this.transform.position.z);
                break;

            case ControlBehaviour.Relative:
                b = this.transform.position + new Vector3(0f, -nextPosition); //ToDO - test this
                break;
        }

        this.transform.position = Vector3.Lerp(a, b, Time.deltaTime * 15f);

        _pitacoRecorder.RecordValue(sensorValue);
    }
}
