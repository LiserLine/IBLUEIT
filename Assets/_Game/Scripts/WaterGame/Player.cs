using System.Collections;
using Ibit.Core.Data;
using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using UnityEngine;

namespace Ibit.WaterGame
{
    public class Player : MonoBehaviour
    {
        /*Events Declaration*/
        public delegate void HaveStarDelegate(int roundScore, int roundNumber);
        public event HaveStarDelegate HaveStarEvent;

        public delegate void EnablePlayDelegate();
        public event EnablePlayDelegate EnablePlayEvent;

        /*Player Variables*/
        public float expiratoryPike;
        public float sensorValue;
        public bool flowStoped;

        /*Utility Variables*/
        public bool stop;
        public bool waitSignal;
        public int x;
        public int _roundNumber;
        Coroutine lastCoroutine;

        private void Start()
        {
            stop = false;
            waitSignal = false;
            x = 0;
            lastCoroutine = null;
            FindObjectOfType<RoundManager>().AuthorizePlayerFlowEvent += ReceivedMessage;
            FindObjectOfType<SerialController>().OnSerialMessageReceived += OnMessageReceived;
            FindObjectOfType<SerialController>().StartSamplingDelayed();
        }

        //Sending HaveStar Event
        protected virtual void OnHaveStar(int roundScore, int roundNumber)
        {
            HaveStarEvent?.Invoke(roundScore, roundNumber);
        }

        //Authorize RoundManager
        protected virtual void OnAuthorize()
        {
            EnablePlayEvent?.Invoke();
        }

        //Flow values being received by the serial controller
        private void OnMessageReceived(string msg)
        {
            if (msg.Length < 1)
                return;

            sensorValue = Parsers.Float(msg);

            if (sensorValue > 0 && expiratoryPike < sensorValue)
                expiratoryPike = sensorValue;
        }

        public void ReceivedMessage(bool hasPlayed, int roundNumber)
        {
            //Se é pra jogar waitSignal = true. Senão waitSignal = false
            waitSignal = hasPlayed;
            _roundNumber = roundNumber;
            Debug.Log(roundNumber);
            if (hasPlayed) lastCoroutine = StartCoroutine(Flow());

            if (!hasPlayed)
            {
                StopCoroutine(lastCoroutine);
                OnHaveStar(0, roundNumber);
            }
        }

        private IEnumerator Flow()
        {
            var picoAtual = expiratoryPike;

            //While player does not blow.
            while (sensorValue <= GameManager.PitacoFlowThreshold)
            {
                Debug.Log($"Player.sensorValue {sensorValue}");
                yield return null;
            }

            //Player is blowing, take the highest value.
            while (sensorValue > GameManager.PitacoFlowThreshold)
            {
                Debug.Log($"Player.sensorValue {sensorValue}");
                picoAtual = expiratoryPike;
                //calculate the percentage of the pike.
                yield return null;
            }

            CalculateFlowPike(picoAtual);
            waitSignal = false;
            OnAuthorize();
        }

        private void CalculateFlowPike(float pikeValue)
        {
            var playerPike = Pacient.Loaded.Capacities.ExpPeakFlow;
            Debug.Log("Pico do Jogador: " + playerPike);

            var percentage = pikeValue / playerPike;
            Debug.Log("Valor do pico máximo: " + pikeValue);
            Debug.Log("Porcentagem: " + percentage);

            if (percentage > 0.75f)
            {
                OnHaveStar(3, _roundNumber);
            }
            else if (percentage > 0.5f)
            {
                OnHaveStar(2, _roundNumber);
            }
            else if (percentage > 0.25f)
            {
                OnHaveStar(1, _roundNumber);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) && waitSignal == false)
            {
                OnAuthorize();
            }
        }
    }
}
