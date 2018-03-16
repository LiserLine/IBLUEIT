using Ibit.Core.Data;
using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using Ibit.Core.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.CakeGame
{
    public class RoundManager : MonoBehaviour
    {
        private bool jogou = true;
        private bool paraTempo;
        private bool partidaCompleta;
        private bool ppasso;
        private int passo;
        private float timer = 10;
        private SerialController sc;

        [SerializeField] private Stars score;
        [SerializeField] public GameObject TextPanel;
        [SerializeField] private Candles candle;
        [SerializeField] private Text displayHowTo, displayTimer;
        [SerializeField] private int[] finalScore = new int[3];
        [SerializeField] private ScoreMenu finalScoreMenu;
        [SerializeField] private Player player;
        
        private void Awake()
        {
            sc = FindObjectOfType<SerialController>();
        }

        private IEnumerator PlayGame()
        {
            while (!partidaCompleta)
            {
                if (ppasso)
                {
                    CleanScene();

                    while (!sc.IsConnected)
                    {
                        passo = -1;
                        TextPanel.SetActive(true);
                        displayHowTo.text = "Seu PITACO não está conectado! Conecte o dispositivo e volte ao menu principal.";
                        yield return null;
                    }

                    switch (passo)
                    {
                        case 1:
                            displayHowTo.text = "Pressione [Enter] e assopre o mais forte que conseguir dentro do tempo.";
                            break;

                        case 2:
                            sc.StartSampling();
                            sc.Recalibrate();

                            displayHowTo.text = "";

                            TextPanel.SetActive(false);

                            while (player.sensorValue <= GameManager.PitacoFlowThreshold * 2 && jogou)
                                yield return null;

                            StopCountdown();
                            paraTempo = true;

                            //saiu do 0
                            while (player.sensorValue > GameManager.PitacoFlowThreshold && jogou)
                            {
                                FlowAction(player.picoExpiratorio);
                                yield return null;
                            }

                            //voltou pro 0
                            finalScoreMenu.pikeString[0] = player.picoExpiratorio.ToString();
                            TextPanel.SetActive(true);

                            if (jogou)
                            {
                                displayHowTo.text = "Parabéns!\nPressione [Enter] para ir para a proxima rodada.";
                                SoundManager.Instance.PlaySound("Success");
                            }

                            player.picoExpiratorio = 0;
                            break;

                        case 3:
                            RestauraVariaveis();
                            break;

                        case 4:
                            displayHowTo.text = "";
                            TextPanel.SetActive(false);
                            while (player.sensorValue <= GameManager.PitacoFlowThreshold * 2 && jogou)
                                yield return null;

                            StopCountdown();
                            paraTempo = true;

                            //saiu do 0
                            while (player.sensorValue > GameManager.PitacoFlowThreshold && jogou)
                            {
                                FlowAction(player.picoExpiratorio);
                                yield return null;
                            }

                            //voltou pro 0
                            finalScoreMenu.pikeString[1] = player.picoExpiratorio.ToString();
                            TextPanel.SetActive(true);

                            if (jogou)
                            {
                                displayHowTo.text = "Parabéns!\nPressione [Enter] para ir para a proxima rodada.";
                                SoundManager.Instance.PlaySound("Success");
                            }

                            player.picoExpiratorio = 0;
                            break;

                        case 5:
                            RestauraVariaveis();
                            break;

                        case 6:
                            displayHowTo.text = "";
                            TextPanel.SetActive(false);
                            while (player.sensorValue <= GameManager.PitacoFlowThreshold * 2 && jogou)
                                yield return null;

                            StopCountdown();
                            paraTempo = true;

                            //saiu do 0
                            while (player.sensorValue > GameManager.PitacoFlowThreshold && jogou)
                            {
                                FlowAction(player.picoExpiratorio);
                                yield return null;
                            }

                            //voltou pro 0
                            finalScoreMenu.pikeString[2] = player.picoExpiratorio.ToString();
                            TextPanel.SetActive(true);

                            if (jogou)
                            {
                                displayHowTo.text = "Parabéns!\nPressione [Enter] para ver a sua pontuação.";
                                SoundManager.Instance.PlaySound("Success");
                            }

                            player.picoExpiratorio = 0;
                            sc.StopSampling();
                            FindObjectOfType<PitacoLogger>().StopLogging();
                            break;
                    }
                    ppasso = false;
                }
                yield return null;
            }
        }

        private void RestauraVariaveis()
        {
            displayHowTo.text = "Pressione [Enter] e assopre o mais forte que conseguir";
            timer = 10;
            jogou = true;
            paraTempo = false;
        }

        #region Calculating Flow Percentage

        public void FlowAction(float flowValue)
        {
            var picoJogador = Pacient.Loaded.Capacities.ExpPeakFlow;
            var percentage = flowValue / picoJogador;

            if (percentage > 0.25f)
            {
                candle.TurnOff(0);
                score.FillStars(0);
                finalScore[(passo / 2) - 1] = 1;
            }
            if (percentage > 0.5f)
            {
                candle.TurnOff(1);
                score.FillStars(1);
                finalScore[(passo / 2) - 1] = 2;
            }
            if (percentage > 0.75f)
            {
                candle.TurnOff(2);
                score.FillStars(2);
                finalScore[(passo / 2) - 1] = 3;
            }
        }

        #endregion Calculatin Flow Percentage

        #region Cleaning Stats

        public void CleanScene()
        {
            candle.TurnOn();
            score.UnfillStars();
        }

        #endregion Cleaning Stats

        #region Step Controllers

        public void ExecuteNextStep()
        {
            ppasso = true;
            passo++;
        }
        #endregion Step Controllers

        #region Countdown Timer

        public void StopCountdown()
        {
            timer = 10;
            displayTimer.text = "";
        }

        #endregion Countdown Timer

        private void Start()
        {
            passo = 0;
            ppasso = false;
            partidaCompleta = false;
            displayHowTo.text = "Pressione [Enter] para começar.";
            StartCoroutine(PlayGame());
        }

        // Update is called once per frame
        private void Update()
        {
            //print(passo);
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ppasso = true;
                passo++;
            }

            if (passo > 6)
            {
                partidaCompleta = true;
                passo = 6;
                displayHowTo.text = "";
            }

            if ((passo == 2 || passo == 4 || passo == 6) && paraTempo == false)
            {
                timer -= Time.deltaTime;
                displayTimer.text = "Timer: " + timer.ToString("f0");
                if (timer <= 0)
                {
                    timer = 0;
                    jogou = false;
                    player.picoExpiratorio = 0;
                    displayHowTo.text = "Ei! Você esqueceu de jogar!...\n[Enter] para continuar";
                    SoundManager.Instance.PlaySound("Failed");
                }
            }

            //ToDo - Isso aqui é executado somente uma vez, mas dentro do update é executado a cada frame
            // Colocar em algum metodo depois para que seja chamado somente uma vez
            if (partidaCompleta)
            {
                //SoundManager.Instance.PlaySound("Finished");
                TextPanel.SetActive(false);
                finalScoreMenu.DisplayFinalScore(finalScore[0], finalScore[1], finalScore[2]);
                finalScoreMenu.ToggleScoreMenu();
            }
        }
    }
}