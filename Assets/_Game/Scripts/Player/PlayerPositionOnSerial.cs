using UnityEngine;

public partial class Player
{
    private void PositionOnSerial(string msg)
    {
        if (msg.Length < 1)
            return;

        var sensorValue = Utils.ParseFloat(msg);

        sensorValue = sensorValue < -GameConstants.PitacoThreshold || sensorValue > GameConstants.PitacoThreshold ? sensorValue : 0f;

        var peak = sensorValue > 0 ? Data.RespiratoryInfo.ExpiratoryPeakFlow : -Data.RespiratoryInfo.InspiratoryPeakFlow;

        var nextPosition = sensorValue * CameraLimits.Boundary / peak;

        nextPosition = Mathf.Clamp(nextPosition, -CameraLimits.Boundary, CameraLimits.Boundary);

        var from = this.transform.position;
        var to = new Vector3(this.transform.position.x, nextPosition, this.transform.position.z);

        this.transform.position = Vector3.Lerp(from, to, Time.deltaTime * 10f);

        /**
         * Old Code
         * if (Behaviour == ControlBehaviour.Absolute)
         * {
         *      to = new Vector3(this.transform.position.x, -nextPosition, this.transform.position.z);
         * }
         * else if (Behaviour == ControlBehaviour.Relative)
         * {
         *      to = this.transform.position + new Vector3(0f, -nextPosition);
         * }
        */
    }
}
