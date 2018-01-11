using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public delegate void ObjectDestroyedHandler(GameObject goner);
    public static event ObjectDestroyedHandler OnObjectDestroyed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnObjectDestroyed?.Invoke(collision.gameObject);
        Destroy(collision.gameObject);
    }
}
