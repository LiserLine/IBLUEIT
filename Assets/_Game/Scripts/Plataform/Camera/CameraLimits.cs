using UnityEngine;

public class CameraLimits : MonoBehaviour
{
    [HideInInspector]
    public static float Boundary;

    public void Awake() => Boundary = Camera.main.orthographicSize * 0.75f;
}