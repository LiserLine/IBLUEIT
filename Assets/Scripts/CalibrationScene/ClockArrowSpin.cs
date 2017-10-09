using UnityEngine;

public class ClockArrowSpin : MonoBehaviour
{
    public bool SpinClock { get; set; }

    public SerialController serialController;
    
    void OnEnable()
    {
        serialController.OnSerialMessageReceived += OnSerialMessageReceived;
    }

    void OnDisable()
    {
        serialController.OnSerialMessageReceived -= OnSerialMessageReceived;
    }

    void OnSerialMessageReceived(string msg)
    {
        if (!SpinClock) return;

        if (!SerialGetOffset.IsUsingOffset) return;

        if (msg.Length < 1) return;

        var snsrVal = GameConstants.ParseSerialMessage(msg) - SerialGetOffset.Offset;

        snsrVal = snsrVal < -GameConstants.PitacoThreshold || snsrVal > GameConstants.PitacoThreshold ? snsrVal : 0f;

        this.transform.Rotate(Vector3.back, snsrVal);
    }
}
