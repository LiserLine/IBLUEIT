using Ibit.Core.Util;
using UnityEngine;

namespace Ibit.CakeGame
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

        private void Start() => FindObjectOfType<Ibit.Core.Serial.SerialController>().OnSerialMessageReceived += OnMessageReceived;
    }
}