using System.Collections;
using System.Collections.Generic;
using Ibit.Core.Data;
using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using Ibit.Core.Audio;
using UnityEngine;
using Ibit.Plataform.Camera;

namespace Ibit.LeavesGame
{
    public class Player : MonoBehaviour
    {
        #region Events

        /*Events Declaration*/
        public delegate void HaveStarDelegate(int roundScore, int roundNumber, float pikeValue);
        public event HaveStarDelegate HaveStarEvent;

        public delegate void EnablePlayDelegate();
        public event EnablePlayDelegate EnablePlayEvent;

        #endregion

        /*Player Variables*/
        public float maximumPeak;
        public float sensorValue;
        public bool flowStoped;

        /*Utility Variables*/
        public bool stop;
        public static bool waitSignal;
        public int x;
        public int _roundNumber;
        Coroutine lastCoroutine;
        public static Dictionary<float, float> playerRespiratoryInfo = new Dictionary<float, float>();


        [SerializeField]
        private Animator _animator;
        private Spawner _spawner;
        private Scorer _scorer;
        public int score = 0;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Start()
        {
            _spawner = FindObjectOfType<Spawner>();
            _scorer = FindObjectOfType<Scorer>();
            stop = false;
            waitSignal = true;
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

        public void ReceivedMessage(bool hasPlayed)
        {
            //Se é pra jogar waitSignal = true. Senão waitSignal = false
            waitSignal = hasPlayed;

            if (hasPlayed)
            {
                this.gameObject.GetComponent<Collider2D>().enabled = true;
                stop = false;
                lastCoroutine = StartCoroutine(Flow());
            }
            if (!hasPlayed)
            {
                this.gameObject.GetComponent<Collider2D>().enabled = false;
                stop = true;
                StopCoroutine(lastCoroutine);
            }
        }
        public void ExecuteNextStep()
        {
            OnAuthorize();
        }

        private IEnumerator Flow()
        {
            while (!stop)
            {
                //Debug.Log("Sensor Value: " + sensorValue + " | Threshold: " + Pacient.Loaded.PitacoThreshold + "| ExpPeakFlow: " + Pacient.Loaded.Capacities.RawExpPeakFlow * 0.3f);
                //if (sensorValue > Pacient.Loaded.PitacoThreshold && sensorValue > Pacient.Loaded.Capacities.RawExpPeakFlow * 0.3f)
                //{
                //    _animator.SetBool("toGreen", true);
                //}
                //else
                //{
                //    _animator.SetBool("toGreen", false);
                //}
                //if (sensorValue > Pacient.Loaded.Capacities.RawExpPeakFlow * 0.4f)
                //{
                //}//VERMELHO


                sensorValue = sensorValue < -Pacient.Loaded.PitacoThreshold || sensorValue > Pacient.Loaded.PitacoThreshold ? sensorValue : 0f;

                var peak = sensorValue > 0 ? Pacient.Loaded.Capacities.ExpPeakFlow * 0.5f : -Pacient.Loaded.Capacities.InsPeakFlow;

                var nextPosition = sensorValue * CameraLimits.Boundary / peak;

                nextPosition = Mathf.Clamp(nextPosition, -CameraLimits.Boundary, CameraLimits.Boundary);

                var from = this.transform.position;
                var to = new Vector3(this.transform.position.x, -nextPosition);

                this.transform.position = Vector3.Lerp(from, to, Time.deltaTime * 15f);

                playerRespiratoryInfo.Add(Time.time, sensorValue);

                yield return null;
            }

            //OnAuthorize();
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && waitSignal == false)
            {
                ExecuteNextStep();
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            SoundManager.Instance.PlaySound("ItemGrab");
            PoolManager.Instance.DestroyObjectPool(other.gameObject);
            _spawner.SpawnObject();
            _scorer.PutScore();
            Debug.Log(score);
        }
    }

}
