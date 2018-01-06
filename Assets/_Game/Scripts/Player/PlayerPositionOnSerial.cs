using UnityEngine;

public partial class Player
{
    private float cameraBounds = 2.5f; //ToDo - get this properlly

    private void PositionOnSerial(string msg)
    {
        if (msg.Length < 1)
            return;

        var sVal = GameUtilities.ParseFloat(msg);

        sVal = sVal < -GameConstants.PitacoThreshold || sVal > GameConstants.PitacoThreshold ? sVal : 0f;

        var limit = sVal > 0 ? playerDto.RespiratoryInfo.ExpiratoryPeakFlow 
            : -playerDto.RespiratoryInfo.InspiratoryPeakFlow;

        var nextPosition = sVal * cameraBounds / limit;

        //Clip position
        if (nextPosition > cameraBounds)
            nextPosition = cameraBounds + 1;
        else if (nextPosition < -cameraBounds)
            nextPosition = -cameraBounds - 1;

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
