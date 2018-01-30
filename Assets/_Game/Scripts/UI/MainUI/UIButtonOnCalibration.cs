using UnityEngine;
using UnityEngine.UI;

public class UIButtonOnCalibration : MonoBehaviour
{
    private Button button;

    private void Awake() => button = GetComponent<Button>();

    private void FixedUpdate() => button.interactable = Pacient.Loaded != null && Pacient.Loaded.CalibrationDone;
}
