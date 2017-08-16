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

        #endregion

        SetPlayerYPos();
    }

    private void OnDestroy()
    {
       
    }

    private void SetPlayerYPos()
    {

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

    #endregion

}
