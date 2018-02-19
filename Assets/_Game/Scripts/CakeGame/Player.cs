using UnityEngine;

namespace _Game.Scripts.CakeGame
{
    public class Player : MonoBehaviour
    {
        public Candles candle;
        public Stat flow;
        public float picoExpiratorio;
        public Stars score;
        public ScoreMenu scoreMenu;
        public float sensorValue;
        public bool stopedFlow;

        private void OnMessageReceived(string msg)
        {
            if (msg.Length < 1)
                return;

            sensorValue = Parsers.Float(msg);

            if (sensorValue > 0 && picoExpiratorio < sensorValue)
                picoExpiratorio = sensorValue;
        }

        private void Start() => FindObjectOfType<SerialController>().OnSerialMessageReceived += OnMessageReceived;
    }
}
