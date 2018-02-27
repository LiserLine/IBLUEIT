using Ibit.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI
{
    public class UnlockOnCalibration : MonoBehaviour
    {
        private void OnEnable() => this.GetComponent<Button>().interactable = Pacient.Loaded != null && Pacient.Loaded.CalibrationDone;
    }
}