using System;
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

    private const float RelativeLimit = 0.3f;

    [Header("Settings")]
    [Range(1f, 100f)]
    public float Sensitivity;
    public ControlBehaviour Behaviour;

    private void Start()
    {
        _transform = GetComponent<Transform>();
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
        newYPos /= 100f / Sensitivity;

        Vector3 a = _transform.position;
        Vector3 b = Vector3.zero;
        switch (Behaviour)
        {
            case ControlBehaviour.Absolute:
                b = new Vector3(_transform.position.x, newYPos * 5f, _transform.position.z);
                break;

            case ControlBehaviour.Relative:
                if (newYPos > RelativeLimit || newYPos < -RelativeLimit)
                {
                    b = _transform.position + new Vector3(0f, newYPos, 0f);
                }
                break;
        }

        _transform.position = Vector3.Lerp(a, b, Time.deltaTime * 10f);

        PitacoRecorder.Instance.Add(_transform.position.y);
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
            PitacoRecorder.Instance.WriteData(plr, new Stage { Id = 123456789, SensitivityUsed = Sensitivity }, GameConstants.GetSessionsPath(plr), true);
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
