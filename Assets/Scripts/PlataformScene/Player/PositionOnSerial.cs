using UnityEngine;

public class PositionOnSerial : MonoBehaviour
{
    private Player _player;
    private Transform _transform;
    private float _cameraOffset;
    private const float RelativeLimit = 0.3f;

    public ControlBehaviour Behaviour;
    public SerialController SerialController;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _cameraOffset = Camera.main.orthographicSize - Camera.main.transform.position.y - 1;
        _player = GameManager.Instance?.Player;
    }

    private void Update()
    {
        ChangeBehaviourHotkey();
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
        PitacoRecorder.Instance.Start();
    }

    private void OnDisable()
    {
        SerialController.OnSerialMessageReceived -= OnSerialMessageReceived;
        PitacoRecorder.Instance.Stop();

#if UNITY_EDITOR
        PitacoRecorder.Instance.WriteData();
#else
        var stage = GameManager.Instance.Stage;
        PitacoRecorder.Instance.WriteData(_player, stage, GameConstants.GetSessionsPath(_player), true);
#endif

    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!SerialGetOffset.IsUsingOffset) return;

        if (msg.Length < 1) return;

        var sensorValue = GameConstants.ParseSerialMessage(msg) - SerialGetOffset.Offset;

        sensorValue = (sensorValue < -GameConstants.PitacoThreshold || sensorValue > GameConstants.PitacoThreshold) ? sensorValue : 0f;

#if UNITY_EDITOR
        var expiratoryPeakFlow = 300f; //debug
        var nextPosition = _cameraOffset * sensorValue / expiratoryPeakFlow * GameConstants.UserPowerMercy;
#else
        Debug.Log($"UserPowerMercy: {GameConstants.UserPowerMercy}");
        var nextPosition = _cameraOffset * sensorValue / _player.ExpiratoryPeakFlow * GameConstants.UserPowerMercy;
#endif

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

        PitacoRecorder.Instance.Add(_transform.position.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }
}
