using UnityEngine;

public class Player_M1 : MonoBehaviour
{
    public Stat flow;
    public Candles candle;
    public Stars score;
    public ScoreMenu scoreMenu;
    public float picoExpiratorio;
    public bool stopedFlow;
    public float sensorValue;

    void Start() => FindObjectOfType<SerialController>().OnSerialMessageReceived += OnMessageReceived;
    
    void OnMessageReceived(string msg)
    {
        if (msg.Length < 1)
            return;

        sensorValue = Parsers.Float(msg);

        if (sensorValue > 0 && picoExpiratorio < sensorValue)
            picoExpiratorio = sensorValue;
    }
}
