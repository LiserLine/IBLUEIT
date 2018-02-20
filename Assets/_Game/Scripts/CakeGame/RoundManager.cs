﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.CakeGame
{
    public class RoundManager : MonoBehaviour
    {
        public Candles candle;
        public Text displayHowTo, displayTimer;
        public int[] finalScore = new int[3];
        public ScoreMenu finalScoreMenu;
        public Stat flow;
        public bool jogou = true;
        public bool paraTempo;
        public bool partidaCompleta;
        public int passo;
        public Player Player;
        public bool ppasso;
        public Stars score;
        public float timer = 10;

        private IEnumerator PlayGame()
        {
            while (!partidaCompleta)
            {
                if (ppasso)
                {
                    CleanScene();

                    switch (passo)
                    {
                        case 1:
                            displayHowTo.text = "Pressione [Enter] e assopre \n o mais forte que conseguir dentro do tempo.";
                            break;

                        case 2:

                            FindObjectOfType<SerialController>().StartSampling();

                            displayHowTo.text = "";

                            while (Player.sensorValue <= GameManager.PitacoFlowThreshold && jogou)
                                yield return null;

                            StopCountdown();
                            paraTempo = true;
                            //saiu do 0

                            while (Player.sensorValue > GameManager.PitacoFlowThreshold && jogou)
                            {
                                //Debug.Log($"Player.sensorValue > GameConstants.PitacoFlowThreshold {Player.sensorValue > GameConstants.PitacoFlowThreshold}");

                                var picoAtual = Player.picoExpiratorio;

                                FlowAction(picoAtual);

                                yield return null;
                            }

                            displayHowTo.text = "Parabéns!\nPressione [Enter] para ir para a proxima rodada.";
                            //voltou pro 0

                            break;

                        case 3:
                            displayHowTo.text = "Pressione [Enter] e assopre o mais forte que conseguir";
                            timer = 10;
                            jogou = true;
                            paraTempo = false;
                            break;

                        case 4:
                            displayHowTo.text = "";
                            while (Player.sensorValue <= GameManager.PitacoFlowThreshold && jogou)
                            {
                                yield return null;
                            }
                            StopCountdown();
                            paraTempo = true;
                            //saiu do 0

                            while (Player.sensorValue > GameManager.PitacoFlowThreshold && jogou)
                            {
                                //Debug.Log($"Player.sensorValue > GameConstants.PitacoFlowThreshold {Player.sensorValue > GameConstants.PitacoFlowThreshold}");

                                var picoAtual = Player.picoExpiratorio;

                                FlowAction(picoAtual);

                                yield return null;
                            }

                            //voltou pro 0
                            displayHowTo.text = "Muito bem!\nPressione [Enter] para continuar.";

                            break;

                        case 5:
                            displayHowTo.text = "Pressione [Enter] e assopre o mais forte que conseguir";
                            timer = 10;
                            jogou = true;
                            paraTempo = false;
                            break;

                        case 6:
                            displayHowTo.text = "";

                            while (Player.sensorValue <= GameManager.PitacoFlowThreshold && jogou)
                                yield return null;

                            StopCountdown();
                            paraTempo = true;
                            //saiu do 0

                            while (Player.sensorValue > GameManager.PitacoFlowThreshold && jogou)
                            {
                                //Debug.Log($"Player.sensorValue > GameConstants.PitacoFlowThreshold {Player.sensorValue > GameConstants.PitacoFlowThreshold}");

                                var picoAtual = Player.picoExpiratorio;

                                FlowAction(picoAtual);

                                yield return null;
                            }

                            //voltou pro 0
                            displayHowTo.text = "Muito bem!\nPressione [Enter] para continuar.";
                            break;
                    }
                    ppasso = false;
                }

                yield return null;
            }
        }

        #region Calculatin Flow Percentage

        public void FlowAction(float flowValue)
        {
            var picoJogador = Pacient.Loaded.Capacities.ExpPeakFlow;
            var percentage = flowValue / picoJogador;
            //Debug.Log(percentage);
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
            flow.CurrentVal = percentage * 100;
        }

        #endregion Calculatin Flow Percentage

        #region Cleaning Stats

        public void CleanScene()
        {
            candle.TurnOn();
            score.UnfillStars();
            flow.CurrentVal = 0;
        }

        #endregion Cleaning Stats

        #region Step Controllers

        public void ExecuteNextStep()
        {
            ppasso = true;
        }

        private void SetNextStep(bool jumpToStep = false)
        {
            passo++;
            ppasso = jumpToStep;
        }

        private void SetStep(int step, bool jumpToStep = false)
        {
            passo = step;
            ppasso = jumpToStep;
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
            displayHowTo.text = "Aperte [ENTER] para começar.";
            flow.Initialize();
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
                displayTimer.text = timer.ToString("f0");
                if (timer <= 0)
                {
                    timer = 0;
                    jogou = false;
                    displayHowTo.text = "Ei! Você esqueceu de jogar!...\n[Enter] para continuar";
                }
            }

            if (partidaCompleta)
            {
                finalScoreMenu.DisplayFinalScore(finalScore[0], finalScore[1], finalScore[2]);
                finalScoreMenu.ToggleScoreMenu();
            }
        }
    }
}