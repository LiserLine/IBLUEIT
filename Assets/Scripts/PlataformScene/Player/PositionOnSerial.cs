using UnityEngine;

public class PositionOnSerial : MonoBehaviour
{
    private Transform _transform;
    private float _cameraOffset;
    private const float RelativeLimit = 0.3f;

    public ControlBehaviour Behaviour;
    public SerialController SerialController;

    private void Start()
    {

#if UNITY_EDITOR
        if (GameManager.Instance.Player == null)
        {
            GameManager.Instance.Player = new Player
            {
                Name = "NetRunner",
                Id = 9999,
                RespiratoryInfo = new RespiratoryInfo
                {
                    InspiratoryPeakFlow = 300f,
                    ExpiratoryPeakFlow = 600f,
                    InspiratoryFlowTime = 1f,
                    ExpiratoryFlowTime = 3f,
                    RespirationFrequency = 6f                    
                },
                CalibrationDone = true,
                SessionsDone = 1,
            };
        }

        if (GameManager.Instance.Stage == null)
        {
            GameManager.Instance.Stage = new Stage
            {
                Id = 777,
            };
        }
#endif

        _transform = GetComponent<Transform>();
        _cameraOffset = Camera.main.orthographicSize - Camera.main.transform.position.y - 1;
    }

    private void Update()
    {
        //ChangeBehaviourHotkey();
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

    private void OnEnable()
    {
        SerialController.OnSerialMessageReceived += OnSerialMessageReceived;
        OnPlataformStageStart(); //ToDo - handle this to an event
    }

    private void OnDisable()
    {
        SerialController.OnSerialMessageReceived -= OnSerialMessageReceived;
        OnPlataformStageEnd(); //ToDo - handle this to an event
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!SerialGetOffset.IsUsingOffset) return;

        if (msg.Length < 1) return;

        var sensorValue = GameConstants.ParseSerialMessage(msg) - SerialGetOffset.Offset;

        sensorValue = (sensorValue < -GameConstants.PitacoThreshold || sensorValue > GameConstants.PitacoThreshold) ? sensorValue : 0f;

        var nextPosition = _cameraOffset * sensorValue / GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow * GameConstants.UserPowerMercy;

        Vector3 a = _transform.position;
        Vector3 b = Vector3.zero;
        switch (Behaviour)
        {
            case ControlBehaviour.Absolute:
                b = new Vector3(_transform.position.x, nextPosition, _transform.position.z);
                break;

            case ControlBehaviour.Relative:
                if (!(nextPosition >= -RelativeLimit && nextPosition <= RelativeLimit))
                {
                    b = _transform.position + new Vector3(0f, nextPosition);
                }
                break;
        }

        _transform.position = Vector3.Lerp(a, b, Time.deltaTime * 15f);

        PitacoRecorder.Instance.AddIncomingData(_transform.position.y);
    }

    private void OnPlataformStageStart()
    {
        PitacoRecorder.Instance.StartRecording();
    }

    private void OnPlataformStageEnd()
    {
        GameManager.Instance.Player.SessionsDone++;
        PitacoRecorder.Instance.StopRecording();
        PitacoRecorder.Instance.WriteData(GameManager.Instance.Player, GameManager.Instance.Stage, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }
}
