using UnityEngine;
using UnityEngine.UI;

public class UnlockOnCalibration : MonoBehaviour
{
    private void OnEnable() => this.GetComponent<Button>().interactable = Pacient.Loaded != null && Pacient.Loaded.CalibrationDone;
}
