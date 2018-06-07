using System.Collections;
using Ibit.Core.Data;
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
        public float maximumPeak;
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
        }

        public void ReceivedMessage(bool hasPlayed, int roundNumber)
        {
            //Se é pra jogar waitSignal = true. Senão waitSignal = false
            waitSignal = hasPlayed;
            _roundNumber = roundNumber;

            if (hasPlayed)
                lastCoroutine = StartCoroutine(Flow());

            if (!hasPlayed)
            {
                StopCoroutine(lastCoroutine);
                OnHaveStar(0, roundNumber, 0);
            }
        }
        public void ExecuteNextStep()
        {
            //Debug.Log("tetris");
            OnAuthorize();
        }
        private IEnumerator Flow()
        {
            //While player does not blow.
            while (sensorValue >= -Pacient.Loaded.PitacoThreshold * 2f)
            {
                //Debug.Log($"Wait: {sensorValue}");
                yield return null;
            }

            //Player is blowing, take the highest value.
            while (sensorValue < -Pacient.Loaded.PitacoThreshold)
            {
                //Debug.Log($"Blow: {sensorValue}");

                if (sensorValue < maximumPeak)
                {
                    maximumPeak = sensorValue;
                    //Debug.Log("Novo pico máximo: " + maximumPeak);
                }

                //calculate the percentage of the pike.
                yield return null;
            }

            SoundManager.Instance.PlaySound("Success");

            CalculateFlowPike(maximumPeak);
            waitSignal = false;
            OnAuthorize();
        }

        private void CalculateFlowPike(float pikeValue)
        {
            var playerPike = -Pacient.Loaded.Capacities.RawInsPeakFlow;
            
            var percentage = -pikeValue / playerPike;
            
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

            maximumPeak = 0f;
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
