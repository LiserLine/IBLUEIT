using UnityEngine;

public class CameraLimits : MonoBehaviour
{
    [HideInInspector]
    public static float Boundary;

    public void Start() => Boundary = Camera.main.orthographicSize * 0.75f;
}
