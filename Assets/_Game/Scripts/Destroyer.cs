using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void Start()
    {
        var distance = Camera.main.orthographicSize * Camera.main.aspect;
        this.gameObject.transform.Translate(-distance - 2f, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Spawner.Instance.ObjectsOnScene.Remove(collision.gameObject);
        Destroy(collision.gameObject);
    }
}
