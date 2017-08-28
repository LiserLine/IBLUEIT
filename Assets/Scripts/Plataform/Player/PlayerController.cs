using System.Collections;
using UnityEngine;

/// <summary>
/// This script is set to the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Transform _transform;
    private SerialListener _serialMessager;
    private bool _isUsingOffset;
    private float _offset;

    [Header("Settings")]
    public float Sensitivity;
    public ControlBehaviour Behaviour;

    private void Start()
    {
        _transform = this.GetComponent<Transform>();
        _serialMessager = GetComponent<SerialListener>();
        _isUsingOffset = false;

        StartCoroutine(GetOffset());
    }

    private void Update()
    {
        #region Running Toggles

        ToggleControlBehaviour();

        #endregion

        SetPlayerPosition();
    }

    private IEnumerator GetOffset()
    {
        yield return new WaitForSeconds(3f);
        var temp = 0f;
        for (var i = 0; i < 5000; i++)
        {
            var message = _serialMessager.MessageReceived;
            temp += ParseSerialMessage(message);
        }
        _offset = temp / 5000f;
        _isUsingOffset = true;
    }

    private void SetPlayerPosition()
    {
        var message = _serialMessager.MessageReceived;
        if (message.Length < 1) return;

        if (!_isUsingOffset) return;

        var newYPos = ParseSerialMessage(message);
        newYPos -= _offset;

        PitacoRecorder.Instance.Add(newYPos);

        newYPos *= (Sensitivity / 10f);

        if (Behaviour == ControlBehaviour.Absolute)
        {
            _transform.position = Vector3.Lerp(_transform.position,
                new Vector3(_transform.position.x, newYPos, _transform.position.z), Time.deltaTime * 10f);
        }
        else
        {
            if (newYPos > 3f || newYPos < -3f)
            {
                //ToDo - relative control
                _transform.position += new Vector3(0f, newYPos * 0.1f, 0f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }

    private void OnEnable()
    {
        PitacoRecorder.Instance.Start();
    }

    private void OnDisable()
    {
        PitacoRecorder.Instance.Stop();

        var plr = GameManager.Instance?.Player;
        if (plr != null)
        {
            PitacoRecorder.Instance.WriteData(plr, new Stage() { Id = 123456789, SensitivityUsed = Sensitivity }, GameConstants.GetSessionsPath(plr), true);
        }
        else
        {
            PitacoRecorder.Instance.WriteData();
        }
    }

    private float ParseSerialMessage(string msg)
    {
        msg = msg.Replace('.', ',');
        return float.Parse(msg);
    }

    #region Toggles

    private void ToggleControlBehaviour()
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

    #endregion

}
