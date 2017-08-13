using UnityEngine;

public class ScrollUV : MonoBehaviour
{
    private MeshRenderer _mr;
    public float BackgroundSpeed = 1f;

    private void Start()
    {
        _mr = gameObject.GetComponent<MeshRenderer>();
    }

    Vector2 offset;
    private void Update()
    {
        offset = _mr.material.mainTextureOffset;
        offset.x += Time.deltaTime / (1f/BackgroundSpeed);
        _mr.material.mainTextureOffset = offset;
    }
}
