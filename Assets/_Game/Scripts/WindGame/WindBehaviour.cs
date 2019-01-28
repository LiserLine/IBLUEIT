using UnityEngine;

namespace Ibit.WindGame
{
    public class WindBehaviour : MonoBehaviour
    {
        public Sprite[] waterLevels;

        private void Start() => FindObjectOfType<Scorer>().ChangeWaterLevelEvent += ChangeWaterSprite;

        private void ChangeWaterSprite(int level)
        {
            // Level 0 -> Pico >= 75% (3 estrelas)
            // Level 1 -> Pico >= 50% (2 estrelas)
            // Level 2 -> Pico >= 25% (1 estrelas)
            // Level 3 -> Pico < 25% (0 estrelas)
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                this.gameObject.transform.Rotate(Vector3.forward * 100);
            }
        }
    }
}