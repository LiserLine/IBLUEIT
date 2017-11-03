using UnityEngine;

public class FlowTester : MonoBehaviour
{
    private PitacoRecorder _pitacoRecorder;

    public SerialController SerialController;

    private void Start()
    {
        SerialController.OnSerialMessageReceived += OnSerialMessageReceived;
        _pitacoRecorder = new PitacoRecorder("Pitaco");
    }

    private void OnEnable()
    {
        _pitacoRecorder.StartRecording();
    }

    private void OnDisable()
    {
        _pitacoRecorder.StopRecording();
        _pitacoRecorder.WriteData(null, null, true);
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (!SerialGetOffset.IsUsingOffset) return;

        if (msg.Length < 1) return;

        var sensorValue = GameUtilities.ParseFloat(msg) - SerialGetOffset.Offset;

        _pitacoRecorder.RecordValue(sensorValue);
    }
}
