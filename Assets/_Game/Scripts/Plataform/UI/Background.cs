using UnityEngine;

namespace Ibit.Plataform.UI
{
    public class Background : MonoBehaviour
    {
        private void Start() => ResizeToCamera();

        private void ResizeToCamera()
        {
            var height = 2f * UnityEngine.Camera.main.orthographicSize;
            var width = height * UnityEngine.Camera.main.aspect;
            this.transform.localScale = new Vector3(width, height);
        }
    }
}