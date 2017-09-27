using UnityEngine;

public class ClockArrowSpin : MonoBehaviour
{
    private SerialListener serialListener;

    [Range(0f,1f)]
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

        this.transform.Rotate(Vector3.forward, snsrVal * threshold);
    }
}
