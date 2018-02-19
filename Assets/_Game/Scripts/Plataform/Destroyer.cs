using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void Awake()
    {
        var distance = Camera.main.orthographicSize * Camera.main.aspect;
        this.gameObject.transform.Translate(-distance - 2f, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision) => Destroy(collision.gameObject);
}