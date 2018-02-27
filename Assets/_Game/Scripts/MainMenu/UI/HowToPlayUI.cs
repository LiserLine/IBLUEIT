using Ibit.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI
{
    public class HowToPlayUI : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private Sprite[] imageList;

        private int iterator;

        private void OnEnable()
        {
            if (Pacient.Loaded.HowToPlayDone)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }

        public void SwitchImage()
        {
            if (iterator == imageList.Length - 1)
                return;

            iterator++;

            if (iterator == imageList.Length - 1)
                this.GetComponentInChildren<Button>().onClick.AddListener(PacientReady);

            image.sprite = imageList[iterator];
        }

        private void PacientReady()
        {
            Pacient.Loaded.HowToPlayDone = true;
            ResetPanel();
        }

        private void ResetPanel()
        {
            iterator = 0;
            image.sprite = imageList[iterator];
            this.GetComponentInChildren<Button>().onClick.RemoveListener(PacientReady);
            HidePanel();
        }

        private void ShowPanel() => this.transform.localScale = Vector3.one;
        private void HidePanel() => this.transform.localScale = Vector3.zero;
    }
}