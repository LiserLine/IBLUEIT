using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.MainMenu.UI
{
    public class UnlockOnCalibration : MonoBehaviour
    {
        private void OnEnable() => this.GetComponent<Button>().interactable = Pacient.Loaded != null && Pacient.Loaded.CalibrationDone;
    }
}