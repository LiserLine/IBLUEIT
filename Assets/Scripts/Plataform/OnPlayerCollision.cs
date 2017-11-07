using UnityEngine;

public class OnPlayerCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Contains("Target"))
        {
            ((PlataformStage)GameManager.Instance.Stage).TargetHit();
        }

        Destroy(collision.gameObject);
    }
}
