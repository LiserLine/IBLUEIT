public partial class Player
{
    private void Animate(string msg)
    {
        if (msg.Length < 1)
            return;

        var f = Parsers.Float(msg);

        f = f < -GameManager.PitacoFlowThreshold || f > GameManager.PitacoFlowThreshold ? f : 0f;

        this.animator.Play(f < 0 ? "Dolphin-Jump" : "Dolphin-Move");
    }
}
