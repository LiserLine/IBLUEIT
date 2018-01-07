using UnityEngine;

public class CameraBoundary : MonoBehaviour
{
    [HideInInspector]
    public static float Limit;

    public void Start()
    {
        Limit = Camera.main.orthographicSize * 0.75f;
    }
}
