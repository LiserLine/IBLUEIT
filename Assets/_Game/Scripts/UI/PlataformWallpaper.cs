using UnityEngine;

public class PlataformWallpaper : MonoBehaviour
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
        switch (Spawner.Instance.SpawnObjects)
        {
            case EnemyType.Targets:
                bgRenderer.material = day;
                break;
            case EnemyType.TargetsAndObstacles:
                bgRenderer.material = afternoon;
                break;
            case EnemyType.Obstacles:
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
