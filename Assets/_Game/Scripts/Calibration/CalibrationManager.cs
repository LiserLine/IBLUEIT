using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace _Game.Scripts.Calibration
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

        private const int flowTimeThreshold = 1000; // In Miliseconds
        private const int respiratoryFrequencyThreshold = 500; //In Milliseconds //ToDo - Test this variable before implementing in CSV

        private bool runNextStep, balloonPanelOpen, acceptingValues, calibrationDone;

        private int currentExerciseCount;
        private float flowMeter;

        private Capacities newCapacities;
        private Stopwatch flowWatch, timerWatch;
        private Dictionary<long, float> samples;
        private CalibrationExercise currentExercise;

        private void Awake()
        {
            serialController = FindObjectOfType<SerialController>();
            serialController.OnSerialMessageReceived += OnSerialMessageReceived;
            newCapacities = new Capacities();
            flowWatch = new Stopwatch();
            timerWatch = new Stopwatch();
            samples = new Dictionary<long, float>();
        }

        private void Start()
        {
            welcomeText.text = "Primeiro, precisamos calibrar a sua respiração. Vamos lá?";
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
                            DudeTalk("Este relógio medirá sua respiração. Aguarde ele ficar verde e RESPIRE TRANQUILAMENTE no PITACO por 30 segundos.");
                            SetNextStep();
                            break;

                        case 3:
                            if (!serialController.IsConnected)
                            {
                                DudeWarnPitacoDisconnected();
                                continue;
                            }

                            serialController.StartSampling();
                            samples.Clear();

                            yield return new WaitForSeconds(1);
                            balloonText.text = "(relaxe e RESPIRE NORMALMENTE por 30 segundos)";

                            AirFlowStart();

                            StartCoroutine(DisplayCountdown(30));
                            while (flowWatch.ElapsedMilliseconds < 30 * 1000)
                                yield return null;

                            AirFlowStop();

                            flowMeter = FlowMath.MeanFlow(samples.ToList());
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
                                DudeWarnUnknownFlow();
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
                            DudeTalk("Agora mediremos sua força. Aguarde o relógio ficar verde e INSPIRE bem FORTE.");
                            SetNextStep();
                            break;

                        case 6:
                            if (!serialController.IsConnected)
                            {
                                DudeWarnPitacoDisconnected();
                                continue;
                            }

#if UNITY_EDITOR
                            serialController.StartSampling();
#endif
                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart();
                            StartCoroutine(DisplayCountdown(10));
                            balloonText.text = "(INSPIRE bem FORTE no PITACO e aguarde o próximo passo)";

                            yield return new WaitForSeconds(10);

                            AirFlowStop();

                            var insCheck = flowMeter;
                            ResetFlowMeter();

                            if (insCheck < -GameManager.PitacoFlowThreshold)
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Success, currentExercise, insCheck);

                                currentExerciseCount++;

                                if (currentExerciseCount == 2)
                                    SetStep(currentStep + 2, true);
                                else
                                    SetNextStep(true);

                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, insCheck);
                                DudeWarnUnknownFlow();
                                break;
                            }

                        case 7:
                            DudeCongratulate();
                            break;

                        case 8:
                            DudeAskAgain();
                            break;

                        #endregion Inspiration Peak

                        #region Expiration Peak

                        case 9:
                            PrepareNextExercise();
                            currentExercise = CalibrationExercise.ExpiratoryPeak;
                            DudeTalk("Agora faremos ao contrário. Aguarde o relógio ficar verde e ASSOPRE bem FORTE.");
                            SetNextStep();
                            break;

                        case 10:
                            if (!serialController.IsConnected)
                            {
                                DudeWarnPitacoDisconnected();
                                continue;
                            }

#if UNITY_EDITOR
                            serialController.StartSampling();
#endif

                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart();
                            StartCoroutine(DisplayCountdown(10));
                            balloonText.text = "(ASSOPRE bem FORTE no PITACO e aguarde o próximo passo)";

                            // Wait for player input
                            yield return new WaitForSeconds(10);

                            AirFlowStop();

                            var expCheck = flowMeter;
                            ResetFlowMeter();

                            if (expCheck > GameManager.PitacoFlowThreshold)
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Success, currentExercise, expCheck);
                                currentExerciseCount++;

                                if (currentExerciseCount == 2)
                                    SetStep(currentStep + 2, true);
                                else
                                    SetNextStep(true);

                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, expCheck);
                                DudeWarnUnknownFlow();
                                break;
                            }

                        case 11:
                            DudeCongratulate();
                            break;

                        case 12:
                            DudeAskAgain();
                            break;

                        #endregion Expiration Peak

                        #region Expiration Time

                        case 13:
                            PrepareNextExercise();
                            currentExercise = CalibrationExercise.ExpiratoryFlow;
                            DudeTalk("Agora vamos medir o tempo. Aguarde o relógio ficar verde, ASSOPRE e mantenha o relógio girando o MÁXIMO de TEMPO possível.");
                            SetNextStep();
                            break;

                        case 14:
                            if (!serialController.IsConnected)
                            {
                                DudeWarnPitacoDisconnected();
                                continue;
                            }

#if UNITY_EDITOR
                            serialController.StartSampling();
#endif
                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart(false);
                            balloonText.text = "(ASSOPRE e mantenha o relógio girando o MÁXIMO de TEMPO possível)";

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

                                if (currentExerciseCount == 2)
                                    SetStep(currentStep + 2, true);
                                else
                                    SetNextStep(true);

                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, flowWatch.ElapsedMilliseconds);
                                DudeWarnUnknownFlow();
                                break;
                            }

                        case 15:
                            DudeCongratulate();
                            break;

                        case 16:
                            DudeAskAgain();
                            break;

                        #endregion Expiration Time

                        #region Inspiration Time

                        case 17:
                            PrepareNextExercise();
                            currentExercise = CalibrationExercise.InspiratoryFlow;
                            DudeTalk("Agora, quando o relógio ficar verde, INSPIRE e mantenha o relógio girando o MÁXIMO de TEMPO possível");
                            SetNextStep();
                            break;

                        case 18:
                            if (!serialController.IsConnected)
                            {
                                DudeWarnPitacoDisconnected();
                                continue;
                            }

#if UNITY_EDITOR
                            serialController.StartSampling();
#endif
                            exerciceCountText.text = $"Exercício: {currentExerciseCount + 1}/3";
                            yield return new WaitForSeconds(0.5f);

                            AirFlowStart(false);
                            balloonText.text = "(INSPIRE e mantenha o relógio girando o MÁXIMO de TEMPO possível)";

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

                                if (currentExerciseCount == 2)
                                    SetStep(currentStep + 2, true);
                                else
                                    SetNextStep(true);

                                continue;
                            }
                            else
                            {
                                FindObjectOfType<CalibrationLogger>().Write(CalibrationExerciseResult.Failure, currentExercise, flowWatch.ElapsedMilliseconds);
                                DudeWarnUnknownFlow();
                                break;
                            }

                        case 19:
                            DudeCongratulate();
                            break;

                        case 20:
                            DudeAskAgain();
                            break;

                        #endregion Inspiration Time

                        #region Ending Steps

                        case 21:
                            exerciceCountText.gameObject.SetActive(false);
                            SoundManager.Instance.PlaySound("Claps");
                            DudeTalk("Ótimo, agora você está pronto para começar a jogar! Bom jogo!");
                            SetNextStep();
                            break;

                        case 22:
                            calibrationDone = true;
                            Pacient.Loaded.CalibrationDone = calibrationDone;
                            Pacient.Loaded.Capacities = newCapacities;
                            PacientDb.Instance.Save();
                            FindObjectOfType<CalibrationLogger>().StopLogging();

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
    }
}