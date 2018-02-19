using UnityEngine;

namespace _Game.Scripts.Calibration
{
    public class ClockArrow : MonoBehaviour
    {
        public bool SpinClock { get; set; }

        private void Awake() => FindObjectOfType<SerialController>().OnSerialMessageReceived += OnSerialMessageReceived;

        private void OnSerialMessageReceived(string msg)
        {
            if (!SpinClock)
                return;

            if (msg.Length < 1)
                return;

            var snsrVal = Parsers.Float(msg);

            snsrVal = snsrVal < -GameManager.PitacoFlowThreshold * 0.3f || snsrVal > GameManager.PitacoFlowThreshold * 0.3f ? snsrVal : 0f;

            this.transform.Rotate(Vector3.back, snsrVal);
        }
    }
}