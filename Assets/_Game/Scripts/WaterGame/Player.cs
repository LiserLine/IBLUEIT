using System.Collections;
using Ibit.Core.Data;
using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using Ibit.Core.Audio;
using UnityEngine;

namespace Ibit.WaterGame
{
    public class Player : MonoBehaviour
    {
        /*Events Declaration*/
        public delegate void HaveStarDelegate(int roundScore, int roundNumber, float pikeValue);
        public event HaveStarDelegate HaveStarEvent;

        public delegate void EnablePlayDelegate();
        public event EnablePlayDelegate EnablePlayEvent;

        /*Player Variables*/
        public float inspiratoryPike;
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
        protected virtual void OnHaveStar(int roundScore, int roundNumber, float pikeValue)
        {
            HaveStarEvent?.Invoke(roundScore, roundNumber, pikeValue);
        }

        //Authorize RoundManager
        protected virtual void OnAuthorize() => EnablePlayEvent?.Invoke();

        //Flow values being received by the serial controller
        private void OnMessageReceived(string msg)
        {
            if (msg.Length < 1)
                return;

            sensorValue = Parsers.Float(msg);

            if (sensorValue > 0 && inspiratoryPike < sensorValue)
                inspiratoryPike = sensorValue;
        }

        public void ReceivedMessage(bool hasPlayed, int roundNumber)
        {
            //Se é pra jogar waitSignal = true. Senão waitSignal = false
            waitSignal = hasPlayed;
            _roundNumber = roundNumber;
            if (hasPlayed) lastCoroutine = StartCoroutine(Flow());

            if (!hasPlayed)
            {
                StopCoroutine(lastCoroutine);
                OnHaveStar(0, roundNumber, 0);
            }
        }
        public void ExecuteNextStep()
        {
            Debug.Log("tetris");
            OnAuthorize();
        }
        private IEnumerator Flow()
        {
            var picoAtual = inspiratoryPike;

            //While player does not blow.
            while (sensorValue <= GameManager.PitacoFlowThreshold * 2f)
            {
                Debug.Log("Sensor Value: " + sensorValue);
                yield return null;
            }

            //Player is blowing, take the highest value.
            while (sensorValue > GameManager.PitacoFlowThreshold)
            {
                //Debug.Log($"Player.sensorValue {sensorValue}");
                picoAtual = inspiratoryPike;
                //calculate the percentage of the pike.
                yield return null;
            }

            SoundManager.Instance.PlaySound("Success");

            CalculateFlowPike(picoAtual);
            waitSignal = false;
            OnAuthorize();
        }

        private void CalculateFlowPike(float pikeValue)
        {
            var playerPike = Pacient.Loaded.Capacities.InsPeakFlow;

            var percentage = Mathf.Abs(pikeValue / playerPike);
            Debug.Log("Valor do pico máximo: " + pikeValue);
            Debug.Log("Porcentagem: " + percentage);

            if (percentage > 0.75f)
            {
                OnHaveStar(3, _roundNumber, pikeValue);
            }
            else if (percentage > 0.5f)
            {
                OnHaveStar(2, _roundNumber, pikeValue);
            }
            else if (percentage > 0.25f)
            {
                OnHaveStar(1, _roundNumber, pikeValue);
            }

            inspiratoryPike = 0;
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && waitSignal == false)
            {
                ExecuteNextStep();
            }
        }
    }
}
