public partial class Player
{
    private void AnimationOnSerial(string msg)
    {
        if (msg.Length < 1)
            return;

        var sensorValue = Utils.ParseFloat(msg);

        sensorValue = sensorValue < -GameMaster.PitacoThreshold || sensorValue > GameMaster.PitacoThreshold ? sensorValue : 0f;

        this.animator.Play(sensorValue < 0 ? "Dolphin-Jump" : "Dolphin-Move");
    }
}
