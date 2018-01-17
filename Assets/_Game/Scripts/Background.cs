using UnityEngine;

public class Background : MonoBehaviour
{
    private void Start() => ResizeToCamera();

    private void ResizeToCamera()
    {
        var height = 2f * Camera.main.orthographicSize;
        var width = height * Camera.main.aspect;
        this.transform.localScale = new Vector3(width, height);
    }
}
