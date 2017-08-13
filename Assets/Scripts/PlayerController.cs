using UnityEngine;

/// <summary>
/// This script is set to the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Transform _transform;

    [Header("Settings")]
    public float Scale;
    public PlayerControlBehaviour PlayerPosition = PlayerControlBehaviour.Absolute;

    private void Start()
    {
        _transform = this.GetComponent<Transform>();
    }

    private void Update()
    {
        #region Running Toggles

        ToggleSetPlayerBehaviour();
        ToggleSerialRequest();

        #endregion

        SetPlayerYPos();
    }

    private void OnDestroy()
    {
        SerialConnectionManager.Instance.DisabeRequest();
    }

    private void SetPlayerYPos()
    {
        if (SerialConnectionManager.Instance.IsRequestEnabled)
        {
            float resultY = -(SerialConnectionManager.Instance.SensorValue * Scale);

            if (PlayerPosition == PlayerControlBehaviour.Absolute)
            {
                _transform.position = Vector3.Lerp(_transform.position,
                    new Vector3(_transform.position.x, resultY, _transform.position.z), Time.deltaTime * 10f);
            }
            else //relative
            {
                if (resultY > 3f || resultY < -3f)
                {
                    _transform.position += new Vector3(0f, resultY * 0.1f, 0f);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarningFormat("{0} has been hit!", collision.gameObject.name);
        Destroy(collision.gameObject);
    }

    #region Toggles

    private void ToggleSetPlayerBehaviour()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PlayerPosition == PlayerControlBehaviour.Absolute)
            {
                PlayerPosition = PlayerControlBehaviour.Relative;
            }
            else if (PlayerPosition == PlayerControlBehaviour.Relative)
            {
                PlayerPosition = PlayerControlBehaviour.Absolute;
            }

            Debug.LogFormat("PlayerPosition: {0}", PlayerPosition);
        }
    }

    private void ToggleSerialRequest()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (SerialConnectionManager.Instance.IsRequestEnabled)
            {
                SerialConnectionManager.Instance.DisabeRequest();
            }
            else
            {
                SerialConnectionManager.Instance.EnableRequest();
            }

            Debug.LogFormat("IsRequestEnabled: {0}", SerialConnectionManager.Instance.IsRequestEnabled);
        }
    }

    #endregion

}
