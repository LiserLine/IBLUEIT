using UnityEngine;

public class ClockArrowSpin : MonoBehaviour
{
    private SerialListener serialListener;

    [Range(0f, GameConstants.PitacoThreshold)]
    public float threshold;

    void Awake()
    {
        serialListener = GetComponent<SerialListener>();
    }

    void OnEnable()
    {
        serialListener.OnSerialMessageReceived += OnSerialMessageReceived;
    }

    void OnDisable()
    {
        serialListener.OnSerialMessageReceived -= OnSerialMessageReceived;
    }

    void OnSerialMessageReceived(string msg)
    {
        if (!SerialGetOffset.IsUsingOffset) return;

        if (msg.Length < 1) return;

        var snsrVal = GameConstants.ParseSerialMessage(msg) - SerialGetOffset.Offset;

        snsrVal = snsrVal < -threshold || snsrVal > threshold ? snsrVal : 0f;

        this.transform.Rotate(Vector3.back, snsrVal);
    }
}
