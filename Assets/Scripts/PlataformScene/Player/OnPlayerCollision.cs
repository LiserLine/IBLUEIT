using UnityEngine;

public class OnPlayerCollision : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }
}
