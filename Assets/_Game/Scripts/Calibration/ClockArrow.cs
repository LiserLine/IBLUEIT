using UnityEngine;

//ToDo - Use an animation for this
public class ClockArrow : MonoBehaviour
{
    public bool SpinClock { get; set; }

    private void Start() => SerialController.Instance.OnSerialMessageReceived += OnSerialMessageReceived;

    private void OnSerialMessageReceived(string msg)
    {
        if (!SpinClock)
            return;

        if (msg.Length < 1)
            return;

        var snsrVal = Utils.ParseFloat(msg);

        snsrVal = snsrVal < -GameMaster.PitacoThreshold * 0.75f || snsrVal > GameMaster.PitacoThreshold * 0.75f ? snsrVal : 0f;

        this.transform.Rotate(Vector3.back, snsrVal);
    }
}
