using UnityEngine;

public partial class Player
{
    private void PositionOnSerial(string msg)
    {
        if (msg.Length < 1)
            return;

        var sensorValue = GameUtilities.ParseFloat(msg);

        sensorValue = sensorValue < -GameConstants.PitacoThreshold || sensorValue > GameConstants.PitacoThreshold ? sensorValue : 0f;

        var limit = sensorValue > 0 ? playerDto.RespiratoryInfo.ExpiratoryPeakFlow : -playerDto.RespiratoryInfo.InspiratoryPeakFlow;

        var nextPosition = sensorValue * CameraBoundary.Limit / limit;

        GameUtilities.Clip(nextPosition, -CameraBoundary.Limit, CameraBoundary.Limit);

        var from = this.transform.position;
        var to = new Vector3(this.transform.position.x, -nextPosition, this.transform.position.z);

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
