using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    private MeshRenderer _mr;
    public float BackgroundSpeed = 1f;

    private void Start()
    {
        _mr = gameObject.GetComponent<MeshRenderer>();
    }

    private Vector2 _offset;
    private void Update()
    {
        _offset = _mr.material.mainTextureOffset;
        _offset.x += Time.deltaTime / (1f/BackgroundSpeed);
        _mr.material.mainTextureOffset = _offset;
    }
}
