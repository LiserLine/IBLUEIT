using UnityEngine;

public class FlowTester : MonoBehaviour
{
    public SerialController SerialController;

    private void Start()
    {
        SerialController.OnSerialMessageReceived += OnSerialMessageReceived;
    }

    private void OnEnable()
    {
        PitacoRecorder.Instance.StartRecording();
    }

    private void OnDisable()
    {
        PitacoRecorder.Instance.StopRecording();
        PitacoRecorder.Instance.WriteData(null, null, true);
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!SerialGetOffset.IsUsingOffset) return;

        if (msg.Length < 1) return;

        var sensorValue = GameUtilities.ParseFloat(msg) - SerialGetOffset.Offset;

        PitacoRecorder.Instance.RecordValue(sensorValue);
    }
}
