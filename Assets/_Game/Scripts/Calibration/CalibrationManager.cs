using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Ibit.Core.Audio;
using Ibit.Core.Data;
using Ibit.Core.Database;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.Calibration
{
    public partial class CalibrationManager : MonoBehaviour
    {
        public static CalibrationExercise CalibrationToLoad = 0;

        private const int FlowTimeThreshold = 2000; //ms
        private const float RespiratoryFrequencyThreshold = 0.05f; //ms
        private const int TimerRespFreq = 60; //seg
        private const int TimerPeakExercise = 8; //seg,

        private bool _acceptingValues;
        private bool _calibrationDone;
        private bool _runStep;

        private CalibrationLogger _calibrationLogger;
        Dictionary<float, float> _capturedSamples;

        private int _currentExerciseCount;
        private int _currentStep = 1; //default: 1

        private float _flowMeter;
        private Stopwatch _flowWatch;

        private SerialController _serialController;

        private Stopwatch _timerWatch;
        private Capacities _tmpCapacities;

        [SerializeField] private CalibrationExercise _currentExercise;
        [BoxGroup("UI")][SerializeField] private Text _dialogText;
        [BoxGroup("UI")][SerializeField] private Text _exerciseCountText;
        [BoxGroup("UI")][SerializeField] private Text _timerText;
        [BoxGroup("UI")][SerializeField] private GameObject _enterButton;

        [SerializeField] private GameObject _clockObject;
        [SerializeField] private GameObject _dudeObject;

        private void Awake()
        {
            _serialController = FindObjectOfType<SerialController>();
            _serialController.OnSerialMessageReceived += OnSerialMessageReceived;
            _tmpCapacities = new Capacities();
            _flowWatch = new Stopwatch();
            _timerWatch = new Stopwatch();
            _capturedSamples = new Dictionary<float, float>();
            _calibrationLogger = new CalibrationLogger();

            _dudeObject.transform.Translate(-Camera.main.orthographicSize * Camera.main.aspect + (_dudeObject.transform.localScale.x / 2f), 0f, 0f);

            if (CalibrationToLoad > 0)
                _currentExercise = CalibrationToLoad;
        }

        private void OnDestroy()
        {
            _serialController.OnSerialMessageReceived -= OnSerialMessageReceived;
        }

        private void Start()
        {
            //DudeTalk("Para começar, pressione ENTER quando o ícone (Enter) aparecer!");
            StartCoroutine(ControlStates());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                NextStep();

            if (Input.GetKeyDown(KeyCode.F2))
                _serialController.Recalibrate();
        }

        private IEnumerator ControlStates()
        {
            _runStep = true;

            while (!_calibrationDone)
            {
                if (_runStep)
                {
                    // Clear screen
                    _enterButton.SetActive(false);
                    DudeClearMessage();

                    // Wait to show next step
                    yield return new WaitForSeconds(0.7f);

                    switch (_currentExercise)
                    {

                        #region Respiratory Frequency

                        case CalibrationExercise.RespiratoryFrequency:

                            switch (_currentStep)
                            {
                                case 1:
                                    DudeTalk("Você deve respirar somente pela boca. Não precisa morder o PITACO. Mantenha o PITACO sempre para baixo. Pressione (Enter) para continuar.");
                                    SetupNextStep();
                                    break;

                                case 2:
                                    DudeTalk($"Neste exercício, você deve RESPIRAR NORMALMENTE por {TimerRespFreq} segundos. Ao apertar (Enter), o relógio ficará verde para você começar o exercício.");
                                    SetupNextStep();
                                    break;

                                case 3:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    _capturedSamples.Clear();
                                    //_serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    yield return new WaitForSeconds(1f);
                                    _dialogText.text = "(relaxe e RESPIRE NORMALMENTE)";

                                    AirFlowEnable();

                                    StartCoroutine(DisplayCountdown(TimerRespFreq));
                                    while (_flowWatch.ElapsedMilliseconds < TimerRespFreq * 1000)
                                        yield return null;

                                    AirFlowDisable();

                                    _flowMeter = FlowMath.RespiratoryRate(_capturedSamples, TimerRespFreq);

                                    if (_flowMeter > RespiratoryFrequencyThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, _flowMeter);
                                        _tmpCapacities.RespiratoryRate = _flowMeter;
                                        SetupNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, _flowMeter);
                                        DudeWarnUnknownFlow();
                                        SetupStep(_currentStep);
                                        break;
                                    }

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Sua média de frequência respiratória é de {(_tmpCapacities.RawRespRate * 60f):F} resp/min." +
                                        " Pressione (Enter) para continuar com os outros exercícios.");
                                    SetupNextStep();
                                    break;

                                case 5:
                                    Pacient.Loaded.Capacities.RespiratoryRate = _tmpCapacities.RawRespRate;
                                    SaveAndQuit();
                                    break;

                                default:
                                    FindObjectOfType<SceneLoader>().LoadScene(0);
                                    break;
                            }
                            break;

                            #endregion

                            #region Inspiratory Peak

                        case CalibrationExercise.InspiratoryPeak:
                            switch (_currentStep)
                            {
                                case 1:
                                    DudeTalk("Neste exercício, você deve PUXAR O AR COM FORÇA. Serão 3 tentativas. Ao apertar (Enter), o relógio ficará verde para você começar o exercício.");
                                    SetupNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    //_serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1f);

                                    AirFlowEnable();
                                    StartCoroutine(DisplayCountdown(TimerPeakExercise));
                                    _dialogText.text = "(PUXE O AR COM FORÇA 1 vez! E aguarde o próximo passo...)";

                                    yield return new WaitForSeconds(TimerPeakExercise);

                                    AirFlowDisable();

                                    var insCheck = _flowMeter;
                                    ResetFlowMeter();

                                    if (insCheck < -Pacient.Loaded.PitacoThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, insCheck);
                                        _currentExerciseCount++;
                                        SetupNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, insCheck);
                                        DudeWarnUnknownFlow();
                                        SetupStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    SetupStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu pico inspiratório é de {FlowMath.ToLitresPerMinute(_tmpCapacities.RawInsPeakFlow):F} L/min." +
                                        " Pressione (Enter) para continuar com os outros exercícios.");
                                    SetupNextStep();
                                    break;

                                case 5:
                                    Pacient.Loaded.Capacities.InsPeakFlow = _tmpCapacities.RawInsPeakFlow;
                                    SaveAndQuit();
                                    break;

                                default:
                                    FindObjectOfType<SceneLoader>().LoadScene(0);
                                    break;
                            }

                            break;

                            #endregion

                            #region Inspiratory Duration

                        case CalibrationExercise.InspiratoryDuration:

                            switch (_currentStep)
                            {
                                case 1:
                                    DudeTalk("Neste exercício, MANTENHA o ponteiro GIRANDO, PUXANDO O AR! Serão 3 tentativas. Ao apertar (Enter), o relógio ficará verde para você começar o exercício.");
                                    SetupNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    //_serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1);

                                    AirFlowEnable(false);
                                    _dialogText.text = "(MANTENHA o ponteiro GIRANDO, PUXANDO O AR!)";

                                    var tmpThreshold = Pacient.Loaded.PitacoThreshold;
                                    Pacient.Loaded.PitacoThreshold = tmpThreshold  * 0.25f;

                                    while (_flowMeter >= -Pacient.Loaded.PitacoThreshold)
                                        yield return null;

                                    _flowWatch.Restart();

                                    while (_flowMeter < -Pacient.Loaded.PitacoThreshold)
                                        yield return null;

                                    AirFlowDisable();
                                    ResetFlowMeter();
                                    Pacient.Loaded.PitacoThreshold = tmpThreshold;

                                    // Check for player input
                                    if (_flowWatch.ElapsedMilliseconds > FlowTimeThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, _flowWatch.ElapsedMilliseconds);

                                        if (_flowWatch.ElapsedMilliseconds > _tmpCapacities.InsFlowDuration)
                                            _tmpCapacities.InsFlowDuration = _flowWatch.ElapsedMilliseconds;

                                        _currentExerciseCount++;

                                        SetupNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, _flowWatch.ElapsedMilliseconds);
                                        DudeWarnUnknownFlow();
                                        SetupStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    SetupStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu tempo de inspiração máximo é de {(_tmpCapacities.RawInsFlowDuration / 1000f):F} segundos." +
                                        " Pressione (Enter) para continuar com os outros exercícios.");
                                    SetupNextStep();
                                    break;

                                case 5:
                                    Pacient.Loaded.Capacities.InsFlowDuration = _tmpCapacities.RawInsFlowDuration;
                                    SaveAndQuit();
                                    break;

                                default:
                                    FindObjectOfType<SceneLoader>().LoadScene(0);
                                    break;
                            }

                            break;

                            #endregion

                            #region Expiratory Peak

                        case CalibrationExercise.ExpiratoryPeak:

                            switch (_currentStep)
                            {
                                case 1:
                                    DudeTalk("Neste exercício, você deve ASSOPRAR FORTE. Serão 3 tentativas. Ao apertar (Enter), o relógio ficará verde para você começar o exercício.");
                                    SetupNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    //_serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1);

                                    AirFlowEnable();
                                    StartCoroutine(DisplayCountdown(TimerPeakExercise));
                                    _dialogText.text = "(ASSOPRE FORTE 1 vez! E aguarde o próximo passo...)";

                                    // Wait for player input
                                    yield return new WaitForSeconds(TimerPeakExercise);

                                    AirFlowDisable();

                                    var expCheck = _flowMeter;
                                    ResetFlowMeter();

                                    if (expCheck > Pacient.Loaded.PitacoThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, expCheck);
                                        _currentExerciseCount++;
                                        SetupNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, expCheck);
                                        DudeWarnUnknownFlow();
                                        SetupStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    SetupStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu pico expiratório é de {FlowMath.ToLitresPerMinute(_tmpCapacities.RawExpPeakFlow):F} L/min." +
                                        " Pressione (Enter) para continuar com os outros exercícios.");
                                    SetupNextStep();
                                    break;

                                case 5:
                                    Pacient.Loaded.Capacities.ExpPeakFlow = _tmpCapacities.RawExpPeakFlow;
                                    SaveAndQuit();
                                    break;

                                default:
                                    FindObjectOfType<SceneLoader>().LoadScene(0);
                                    break;
                            }

                            break;

                            #endregion

                            #region Expiratory Duration

                        case CalibrationExercise.ExpiratoryDuration:

                            switch (_currentStep)
                            {
                                case 1:
                                    DudeTalk("Neste exercício, MANTENHA o ponteiro GIRANDO, ASSOPRANDO! Serão 3 tentativas. Ao apertar (Enter), o relógio ficará verde para você começar o exercício.");
                                    SetupNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    //_serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1);

                                    AirFlowEnable(false);
                                    _dialogText.text = "(MANTENHA o ponteiro GIRANDO, ASSOPRANDO!)";

                                    var tmpThreshold = Pacient.Loaded.PitacoThreshold;
                                    Pacient.Loaded.PitacoThreshold = tmpThreshold * 0.25f; //this helps the player expel all his air

                                    // Wait for player input to be greather than threshold
                                    while (_flowMeter <= Pacient.Loaded.PitacoThreshold)
                                        yield return null;

                                    _flowWatch.Restart();

                                    while (_flowMeter > Pacient.Loaded.PitacoThreshold)
                                        yield return null;

                                    AirFlowDisable();
                                    ResetFlowMeter();

                                    Pacient.Loaded.PitacoThreshold = tmpThreshold;

                                    // Check for player input
                                    if (_flowWatch.ElapsedMilliseconds > FlowTimeThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, _flowWatch.ElapsedMilliseconds);

                                        if (_flowWatch.ElapsedMilliseconds > _tmpCapacities.ExpFlowDuration)
                                            _tmpCapacities.ExpFlowDuration = _flowWatch.ElapsedMilliseconds;

                                        _currentExerciseCount++;
                                        SetupNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, _flowWatch.ElapsedMilliseconds);
                                        DudeWarnUnknownFlow();
                                        SetupStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    SetupStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu tempo de fluxo expiratório máximo é de {(_tmpCapacities.RawExpFlowDuration / 1000f):F} segundos." +
                                        " Pressione (Enter) para continuar com os outros exercícios.");
                                    SetupNextStep();
                                    break;

                                case 5:
                                    Pacient.Loaded.Capacities.ExpFlowDuration = _tmpCapacities.RawExpFlowDuration;
                                    SaveAndQuit();
                                    break;

                                default:
                                    FindObjectOfType<SceneLoader>().LoadScene(0);
                                    break;
                            }

                            break;

                            #endregion

                    }

                    _enterButton.SetActive(true);
                    _runStep = false;
                }

                yield return null;
            }
        }

        private IEnumerator DisplayCountdown(long timer)
        {
            timer *= 1000;
            _timerWatch.Restart();
            while (_timerWatch.ElapsedMilliseconds < timer)
            {
                yield return null;
                _timerText.text = $"TIMER: {(timer - _timerWatch.ElapsedMilliseconds) / 1000}";
            }
            _timerText.text = "";
        }

        private void ResetFlowMeter() => _flowMeter = 0f;

        private void AirFlowDisable()
        {
            _flowWatch.Stop();

            //FindObjectOfType<PitacoLogger>().Pause(true);

            _clockObject.GetComponent<SpriteRenderer>().color = Color.white;
            _clockObject.GetComponentInChildren<ClockArrowAnimation>().SpinClock = false;
            _acceptingValues = false;
        }

        private void AirFlowEnable(bool restartWatch = true)
        {
            if (restartWatch)
                _flowWatch.Restart();

            //FindObjectOfType<PitacoLogger>().Pause(false);

            _clockObject.GetComponent<SpriteRenderer>().color = Color.green;
            _clockObject.GetComponentInChildren<ClockArrowAnimation>().SpinClock = true;
            _acceptingValues = true;
        }

        private void SaveAndQuit()
        {
            Pacient.Loaded.CalibrationDone = Pacient.Loaded.IsCalibrationDone;
            PacientDb.Instance.Save();
            FindObjectOfType<PitacoLogger>().StopLogging();
            _calibrationLogger.Save();
            FindObjectOfType<SceneLoader>().LoadScene(0);
        }

        private void SerialDisconnectedWarning()
        {
            _enterButton.SetActive(true);
            DudeWarnPitacoDisconnected();
            SetupStep(99);
        }
    }
}