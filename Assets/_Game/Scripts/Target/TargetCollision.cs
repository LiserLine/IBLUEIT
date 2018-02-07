using UnityEngine;

public partial class Target
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.tag.Contains("Player"))
            return;

        Spawner.Instance.SpawnedObjects.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
