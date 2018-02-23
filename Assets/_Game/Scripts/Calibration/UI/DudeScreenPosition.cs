using UnityEngine;

namespace Assets._Game.Scripts.Calibration.UI
{
    public class DudeScreenPosition : MonoBehaviour
    {
        private void Awake()
        {
            var distance = Camera.main.orthographicSize * Camera.main.aspect;
            this.gameObject.transform.Translate(-distance + (this.gameObject.transform.localScale.x / 2f), 0f, 0f);
        }
    }
}