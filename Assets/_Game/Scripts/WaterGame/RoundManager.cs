using System.Collections;
using Ibit.Core.Audio;
using Ibit.Core.Serial;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.WaterGame
{
    public class RoundManager : MonoBehaviour
    {

        /*Events Declaration*/
        public delegate void PlayerFlowDelegate (bool hasPlayed, int roundNumber);
        public event PlayerFlowDelegate AuthorizePlayerFlowEvent;

        public delegate void FinalScoreDelegate ();
        public event FinalScoreDelegate ShowFinalScoreEvent;

        public delegate void CleanRoundDelegate ();
        public event CleanRoundDelegate CleanRoundEvent;

        /*RoundManager Variables*/
        [SerializeField] private Text displayHowTo, displayTimer;
        [SerializeField] private GameObject TextPanel;

        private bool playable, finished, toBackup;
        [SerializeField] private int state, backupState, _roundNumber;
        private float countdownTimer;
        private SerialController sc;

        private void Awake ()
        {
            sc = FindObjectOfType<SerialController> ();
        }

        private void Start ()
        {
            state = 1; // Player start point on State Machine
            finished = false; //To Verify if the player have finished the game
            playable = true; //To keep player at state
            toBackup = false; //Use old state value(Player haven't played->default state->continue to next state)
            countdownTimer = 10; //Time the player has to play(Flow only)
            _roundNumber = 0; //Defines in which round the the player is.
            FindObjectOfType<Player> ().EnablePlayEvent += NotPlayable;
            StartCoroutine (PlayGame ()); //Starts the Gameplay State Machine
        }

        //Sending Event Area
        protected virtual void EnablePlayerFlow (bool hasPlayed, int roundNumber)
        {
            AuthorizePlayerFlowEvent?.Invoke (hasPlayed, roundNumber);
        }

        protected virtual void CleanRound ()
        {
            CleanRoundEvent?.Invoke ();
        }

        protected virtual void ShowFinalScore ()
        {
            ShowFinalScoreEvent?.Invoke ();
        }

        private void NotPlayable ()
        {
            playable = false;
            if (toBackup)
            {
                state = backupState;
                toBackup = false;
            }
            state++;
        }

        private void NotPlayedState ()
        {
            playable = false;
            backupState = state;
            toBackup = true;
            state = 99; // Player haven't played -> default state
        }

        //Start the Countdown Timer
        private void StartCountdown ()
        {
            countdownTimer -= Time.deltaTime;
            displayTimer.text = "Timer: " + countdownTimer.ToString ("f0");
        }

        //Stop the Countdown Timer
        private void ResetCountDown ()
        {
            countdownTimer = 10;
            displayTimer.text = "Timer: 10";
        }

        private void PlayerWakeUp ()
        {
            displayHowTo.text = "Ei! Você esqueceu de jogar!...\n Aperte [Enter] para continuar";
            //Se não jogou mandar sinal false para a permissão do jogador.
            EnablePlayerFlow (false, _roundNumber - 1);
            ResetCountDown ();
            NotPlayedState ();
        }

        #region Gameplay State Machine

        //Incremental states(put them into the correct order)
        private IEnumerator PlayGame ()
        {
            while (!finished)
            {
                while (!sc.IsConnected)
                {
                    state = -1;
                    TextPanel.SetActive (true);
                    displayHowTo.text = "Seu PITACO não está conectado! Conecte o dispositivo e volte ao menu principal.";
                    yield return null;
                }

                switch (state)
                {
                    case 1: //Introduction
                        displayHowTo.text = "Bem-Vindo ao jogo do copo d'agua![ENTER]";

                        while (playable)
                            yield return null;

                        playable = true;
                        break;
                    case 2:
                    case 4:
                    case 6: //Pre-flow
                        sc.Recalibrate ();
                        sc.StartSamplingDelayed ();
                        TextPanel.SetActive (true);
                        displayHowTo.text = "Pressione [Enter] e INSPIRE \n o mais forte que conseguir dentro do tempo.";
                        while (playable)
                            yield return null;
                        CleanRound ();
                        playable = true;
                        TextPanel.SetActive (false);
                        break;
                    case 3:
                    case 5:
                    case 7: //Player's Flow
                        displayHowTo.text = "";
                        EnablePlayerFlow (true, _roundNumber);
                        _roundNumber++;

                        while (playable)
                        {
                            StartCountdown ();
                            yield return null;
                        }

                        ResetCountDown ();
                        playable = true;
                        break;
                    case 8:
                        sc.StopSampling ();
                        TextPanel.SetActive (true);
                        displayHowTo.text = "Pressione [Enter] para visualizar sua pontuação.";
                        Debug.Log("Saving minigame data...");
                        FindObjectOfType<Core.MinigameLogger>().Save();                        
                        break;
                    case 9:
                        TextPanel.SetActive (false);
                        ShowFinalScore ();
                        break;
                    case 99:
                        TextPanel.SetActive (true);
                        while (playable)
                            yield return null;
                        playable = true;
                        break;
                }
                yield return null;
            }
        }

        #endregion;

        private void Update ()
        {
            if (countdownTimer <= 0)
            {
                SoundManager.Instance.PlaySound ("Failed");
                PlayerWakeUp ();
            }
        }
    }
}