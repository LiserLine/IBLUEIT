using UnityEngine;

/// <summary>
/// This script is set to the MainCamera
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private Transform _transform;

    public Transform ObjectToFollow;

    // Use this for initialization
    private void Awake()
    {
        _transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        _transform.position = new Vector3(ObjectToFollow.position.x + 10f, 0f, -25f);
    }
}
