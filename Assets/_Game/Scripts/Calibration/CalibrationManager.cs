using Ibit.Core.Audio;
using Ibit.Core.Data;
using Ibit.Core.Database;
using Ibit.Core.Game;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using Ibit.Plataform.Logger;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Ibit.Calibration
{
    public partial class CalibrationManager : MonoBehaviour
    {
        private SerialController serialController;

        [BoxGroup("UI")]
        [SerializeField]
        private Text welcomeText, balloonText, exerciceCountText, timerText;

        [BoxGroup("UI")]
        [SerializeField]
        private GameObject enterButton, enterButtonSmall, welcomePanel, balloonPanel, resultPanel;

        [BoxGroup("Calibration")]
        [SerializeField]
        private GameObject dude, clock;

        private int currentStep = 1; //default: 1

        private const int flowTimeThreshold = 1000; //ms
        private const int respiratoryFrequencyThreshold = 500; //ms

        private bool runNextStep, balloonPanelOpen, acceptingValues, calibrationDone;

        private int currentExerciseCount;
        private float flowMeter;

        private Capacities newCapacities;
        private Stopwatch flowWatch, timerWatch;
        private Dictionary<long, float> capturedSamples;
        private CalibrationExercise currentExercise;

        private void Awake()
        {
            serialController = FindObjectOfType<SerialController>();
            serialController.OnSerialMessageReceived += OnSerialMessageReceived;
            newCapacities = new Capacities();
            flowWatch = new Stopwatch();
            timerWatch = new Stopwatch();
            capturedSamples = new Dictionary<long, float>();
        }

        private void Start()
        {
            welcomeText.text = "Aqui iremos calibrar o jogo com sua respiração. Para avançar, pressione ENTER quando o ícone (►) aparecer!";
            enterButton.SetActive(true);
            StartCoroutine(ControlStates());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                NextStep();
        }

        private IEnumerator ControlStates()
        {
            while (!calibrationDone)
            {
                if (runNextStep)
                {
                    #region Preparation steps

                    // Clear screen
                    welcomeText.text = "";
                    enterButton.SetActive(false);
                    DudeClearMessage();

                    // Wait 1 sec to show next step
                    yield return new WaitForSeconds(1);

                    // Open Scene Animation
                    // Opens on step 3 or greater
                    if (currentStep >= 2 && !balloonPanelOpen)
                    {
                        // Fade black screen and wait 1s
                        welcomePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                        yield return new WaitForSeconds(1);
                        welcomePanel.SetActive(false);
                        Destroy(enterButton);

                        // Enable dude and clock
                        clock.SetActive(true);
                        dude.SetActive(true);
                        balloonPanel.SetActive(true);

                        enterButton = enterButtonSmall; // Should it be handled this way? I tried to scale in many other ways but no success

                        balloonPanelOpen = true;
                    }

                    switch (currentStep)
                    {
                        case 1:
                            welcomeText.text = "Neste jogo, você deve respirar somente pela boca. Não precisa morder o PITACO.";
                            SetNextStep();
                            break;

                        #endregion Preparation steps

                        #region Respiratory Frequency

                        case 2:
                            currentExercise = CalibrationExercise.RespiratoryFrequency;
                            ResetExerciseCounter();
                            ResetFlowMeter();
                            DudeTalk("Neste exercício, você deve RESPIRAR NORMALMENTE por 30 segundos. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                            SetNextStep();
                            break;

                        case 3:
                            if (!serialController.IsConnected)
                            {
                                SerialDisconnectedWarning();
                                continue;
                            }

                            serialController.StartSampling();
                            capturedSamples.Clear();

                            yield return new WaitForSeconds(1);
                            balloonText.text = "(relaxe e RESPIRE NORMALMENTE)";

                            AirFlowStart();

                            StartCoroutine(DisplayCountdown(32));
                            while (flowWatch.ElapsedMilliseconds < 32 * 1000)
                                yield return null;

                            AirFlowStop();

                            flowMeter = FlowMath.MeanFlow(capturedSamples);
                            Debug.Log($"Respiratory Frequency: {flowMeter}");

                            if (flowMeter > respiratoryFrequencyThreshold)
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Success, currentExercise, flowMeter);
                                newCapacities.RespCycleDuration = flowMeter;
                                SetNextStep(true);
                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, flowMeter);
                                DudeWarnUnknownFlow(); SetStep(currentStep);
                                break;
                            }

                        case 4:
                            SoundManager.Instance.PlaySound("Success");
                            DudeTalk("Muito bem! Agora, vamos continuar com os outros exercícios.");
                            SetNextStep();
                            break;

                        #endregion Respiratory Frequency

                        #region Inspiration Peak

                        case 5:
                            currentExercise = CalibrationExercise.InspiratoryPeak;
                            PrepareNextExercise();
                            DudeTalk("Neste exercício, você deve INSPIRAR FORTE. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                            SetNextStep();
                            break;

                        case 6:
                            if (!serialController.IsConnected)
                            {
                                SerialDisconnectedWarning();
                                continue;
                            }
#if UNITY_EDITOR
                            serialController.StartSampling();
#endif
                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart();
                            StartCoroutine(DisplayCountdown(10));
                            balloonText.text = "(INSPIRE FORTE e aguarde o próximo passo)";

                            yield return new WaitForSeconds(10);

                            AirFlowStop();

                            var insCheck = flowMeter;
                            ResetFlowMeter();

                            if (insCheck < -GameManager.PitacoFlowThreshold)
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Success, currentExercise, insCheck);
                                currentExerciseCount++;
                                SetNextStep(true);
                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, insCheck);
                                DudeWarnUnknownFlow();
                                SetStep(currentStep);
                                break;
                            }

                        case 7:
                            DudeCongratulate();
                            SetStep(currentExerciseCount == 3 ? currentStep + 1 : currentStep - 1);
                            break;

                        #endregion Inspiration Peak

                        #region Expiration Peak

                        case 8:
                            PrepareNextExercise();
                            currentExercise = CalibrationExercise.ExpiratoryPeak;
                            DudeTalk("Neste exercício, você deve ASSOPRAR FORTE. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                            SetNextStep();
                            break;

                        case 9:
                            if (!serialController.IsConnected)
                            {
                                SerialDisconnectedWarning();
                                continue;
                            }

#if UNITY_EDITOR
                            serialController.StartSampling();
#endif

                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart();
                            StartCoroutine(DisplayCountdown(10));
                            balloonText.text = "(ASSOPRE FORTE e aguarde o próximo passo)";

                            // Wait for player input
                            yield return new WaitForSeconds(10);

                            AirFlowStop();

                            var expCheck = flowMeter;
                            ResetFlowMeter();

                            if (expCheck > GameManager.PitacoFlowThreshold)
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Success, currentExercise, expCheck);
                                currentExerciseCount++;
                                SetNextStep(true);
                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, expCheck);
                                DudeWarnUnknownFlow();
                                SetStep(currentStep);
                                break;
                            }

                        case 10:
                            DudeCongratulate();
                            SetStep(currentExerciseCount == 3 ? currentStep + 1 : currentStep - 1);
                            break;

                        #endregion Expiration Peak

                        #region Expiration Time

                        case 11:
                            PrepareNextExercise();
                            currentExercise = CalibrationExercise.ExpiratoryDuration;
                            DudeTalk("Neste exercício, ASSOPRE e mantenha o relógio girando o MÁXIMO QUE PUDER! Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                            SetNextStep();
                            break;

                        case 12:
                            if (!serialController.IsConnected)
                            {
                                SerialDisconnectedWarning();
                                continue;
                            }

#if UNITY_EDITOR
                            serialController.StartSampling();
#endif
                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart(false);
                            balloonText.text = "(ASSOPRE e mantenha o relógio girando o MÁXIMO QUE PUDER)";

                            // Wait for player input to be greather than threshold
                            while (flowMeter <= GameManager.PitacoFlowThreshold)
                                yield return null;

                            flowWatch.Restart();

                            while (flowMeter > GameManager.PitacoFlowThreshold * 0.3f)
                                yield return null;

                            AirFlowStop();

                            Debug.Log($"Expiration Time: {flowWatch.ElapsedMilliseconds} ms ({flowWatch.ElapsedMilliseconds / 1000} secs)");

                            ResetFlowMeter();

                            // Check for player input
                            if (flowWatch.ElapsedMilliseconds > flowTimeThreshold)
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Success, currentExercise, flowWatch.ElapsedMilliseconds);

                                if (flowWatch.ElapsedMilliseconds > newCapacities.ExpFlowDuration)
                                    newCapacities.ExpFlowDuration = flowWatch.ElapsedMilliseconds;

                                currentExerciseCount++;
                                SetNextStep(true);
                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, flowWatch.ElapsedMilliseconds);
                                DudeWarnUnknownFlow();
                                SetStep(currentStep);
                                break;
                            }

                        case 13:
                            DudeCongratulate();
                            SetStep(currentExerciseCount == 3 ? currentStep + 1 : currentStep - 1);
                            break;

                        #endregion Expiration Time

                        #region Inspiration Time

                        case 14:
                            PrepareNextExercise();
                            currentExercise = CalibrationExercise.InspiratoryDuration;
                            DudeTalk("Neste exercício, INSPIRE e mantenha o relógio girando o MÁXIMO QUE PUDER! Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                            SetNextStep();
                            break;

                        case 15:
                            if (!serialController.IsConnected)
                            {
                                SerialDisconnectedWarning();
                                continue;
                            }

#if UNITY_EDITOR
                            serialController.StartSampling();
#endif
                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart(false);
                            balloonText.text = "(INSPIRE para manter o relógio girando o MÁXIMO QUE PUDER)";

                            while (flowMeter >= -GameManager.PitacoFlowThreshold)
                                yield return null;

                            flowWatch.Restart();

                            while (flowMeter < -GameManager.PitacoFlowThreshold * 0.3f)
                                yield return null;

                            AirFlowStop();

                            Debug.Log($"Inspiration Time: {flowWatch.ElapsedMilliseconds} ms ({flowWatch.ElapsedMilliseconds / 1000} secs)");

                            ResetFlowMeter();

                            // Check for player input
                            if (flowWatch.ElapsedMilliseconds > flowTimeThreshold)
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Success, currentExercise, flowWatch.ElapsedMilliseconds);
                                if (flowWatch.ElapsedMilliseconds > newCapacities.InsFlowDuration)
                                    newCapacities.InsFlowDuration = flowWatch.ElapsedMilliseconds;

                                currentExerciseCount++;
                                SetNextStep(true);
                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, flowWatch.ElapsedMilliseconds);
                                DudeWarnUnknownFlow();
                                SetStep(currentStep);
                                break;
                            }

                        case 16:
                            DudeCongratulate();
                            SetStep(currentExerciseCount == 3 ? currentStep + 1 : currentStep - 1);
                            break;

                        #endregion Inspiration Time

                        #region Ending Steps

                        case 17:
                            exerciceCountText.gameObject.SetActive(false);
                            SoundManager.Instance.PlaySound("Claps");
                            DudeTalk("Ótimo, agora você está pronto para começar a jogar! Bom jogo!");
                            SetNextStep();
                            break;

                        case 18:
                            calibrationDone = true;
                            Pacient.Loaded.CalibrationDone = calibrationDone;
                            Pacient.Loaded.Capacities = newCapacities;
                            PacientDb.Instance.Save();
                            FindObjectOfType<CalibrationLogger>().StopLogging();
                            FindObjectOfType<PitacoLogger>().StopLogging();

                            clock.SetActive(false);
                            dude.SetActive(false);
                            balloonPanel.SetActive(false);
                            resultPanel.SetActive(true);
                            break;

                        #endregion Ending Steps

                        case 99: // Quit Scene
                            FindObjectOfType<SceneLoader>().LoadScene(0);
                            break;
                    }

                    enterButton.SetActive(true); // Enable enter button sprite on break
                    runNextStep = false; // Wait for player next command
                }

                yield return null;
            }
        }

        private IEnumerator DisplayCountdown(long timer)
        {
            timer *= 1000;
            timerWatch.Restart();
            while (timerWatch.ElapsedMilliseconds < timer)
            {
                yield return null;
                timerText.text = $"TIMER: {(timer - timerWatch.ElapsedMilliseconds) / 1000}";
            }
            timerText.text = "";
        }

        private void ResetExerciseCounter() => currentExerciseCount = 0;

        private void ResetFlowMeter() => flowMeter = 0f;

        private void PrepareNextExercise()
        {
            ResetExerciseCounter();
            ResetFlowMeter();

            if (serialController.IsConnected)
                serialController.Recalibrate();
        }

        private void AirFlowStart(bool restartWatch = true)
        {
            if (restartWatch)
                flowWatch.Restart();

            clock.GetComponent<SpriteRenderer>().color = Color.green;
            clock.GetComponentInChildren<ClockArrow>().SpinClock = true;
            acceptingValues = true;
        }

        private void AirFlowStop()
        {
            flowWatch.Stop();
            clock.GetComponent<SpriteRenderer>().color = Color.white;
            clock.GetComponentInChildren<ClockArrow>().SpinClock = false;
            acceptingValues = false;
        }

        private void SerialDisconnectedWarning()
        {
            enterButton.SetActive(true);
            DudeWarnPitacoDisconnected();
            SetStep(99);
        }
    }
}