using UnityEngine;

public class Scorer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Contains("Obstacle"))
        {
            ((PlataformStage)GameManager.Instance.Stage).ObstacleAvoided();
        }

        Destroy(collision.gameObject);
    }
}
