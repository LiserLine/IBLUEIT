using UnityEngine;

namespace Ibit.MainMenu.UI
{
    public class HowToPlayUI : MonoBehaviour
    {
        private void OnEnable()
        {
            if (Pacient.Loaded.HowToPlayDone)
                this.gameObject.SetActive(false);
        }

        public void PacientReady() => Pacient.Loaded.HowToPlayDone = true;
    }
}