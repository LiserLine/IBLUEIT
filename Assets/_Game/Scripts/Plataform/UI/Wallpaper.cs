using UnityEngine;

public class Wallpaper : MonoBehaviour
{
    private Vector2 _offset;
    private MeshRenderer _mr;
    private Renderer bgRenderer;

    [SerializeField]
    private bool scroll;

    [SerializeField]
    private float scrollSpeed = 0.1f;

    [SerializeField]
    private Material day;

    [SerializeField]
    private Material afternoon;

    [SerializeField]
    private Material night;

    private void Awake()
    {
        bgRenderer = this.GetComponent<Renderer>();
        _mr = this.GetComponent<MeshRenderer>();
    }

    private void Start() => SwitchBackground();

    private void SwitchBackground()
    {
        switch (Stage.Loaded.SpawnObject)
        {
            case SpawnObject.Targets:
                bgRenderer.material = day;
                break;
            case SpawnObject.TargetsAndObstacles:
                bgRenderer.material = afternoon;
                break;
            case SpawnObject.Obstacles:
                bgRenderer.material = night;
                break;
        }
    }

    private void Update() => Scroll();

    private void Scroll()
    {
        _offset = _mr.material.mainTextureOffset;
        _offset.x += Time.deltaTime / (1f / scrollSpeed);
        _mr.material.mainTextureOffset = _offset;
    }
}
