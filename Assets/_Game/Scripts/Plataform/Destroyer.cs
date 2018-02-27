using UnityEngine;

namespace Ibit.Plataform
{
    public class Destroyer : MonoBehaviour
    {
        private void Awake()
        {
            var distance = UnityEngine.Camera.main.orthographicSize * UnityEngine.Camera.main.aspect;
            this.gameObject.transform.Translate(-distance - 2f, 0f, 0f);
        }

        private void OnTriggerEnter2D(Collider2D collision) => Destroy(collision.gameObject);
    }
}