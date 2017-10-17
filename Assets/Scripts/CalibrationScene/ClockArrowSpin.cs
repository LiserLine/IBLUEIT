using UnityEngine;

public class ClockArrowSpin : MonoBehaviour
{
    public bool SpinClock { get; set; }

    public SerialController serialController;

    private void OnEnable()
    {
        serialController.OnSerialMessageReceived += OnSerialMessageReceived;
    }

    private void OnDisable()
    {
        serialController.OnSerialMessageReceived -= OnSerialMessageReceived;
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!SpinClock) return;

        if (!SerialGetOffset.IsUsingOffset) return;

        if (msg.Length < 1) return;

        var snsrVal = GameUtilities.ParseSerialMessage(msg) - SerialGetOffset.Offset;

        snsrVal = snsrVal < -GameConstants.PitacoThreshold || snsrVal > GameConstants.PitacoThreshold ? snsrVal : 0f;

        this.transform.Rotate(Vector3.back, snsrVal);
    }
}
