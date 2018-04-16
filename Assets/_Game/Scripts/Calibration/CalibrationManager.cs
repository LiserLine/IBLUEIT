using Ibit.Core.Audio;
using Ibit.Core.Data;
using Ibit.Core.Database;
using Ibit.Core.Serial;
using Ibit.Core.Util;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.Calibration
{
    public partial class CalibrationManager : MonoBehaviour
    {
        public static CalibrationExercise CalibrationToLoad = 0;

        private const int FlowTimeThreshold = 1000; //ms
        private const int RespiratoryFrequencyThreshold = 500; //ms

        private bool _acceptingValues;
        private bool _calibrationDone;
        private bool _runStep;

        private CalibrationLogger _calibrationLogger;
        private Dictionary<long, float> _capturedSamples;

        private int _currentExerciseCount;
        private int _currentStep = 1; //default: 1

        private float _flowMeter;
        private Stopwatch _flowWatch;

        private SerialController _serialController;

        private Stopwatch _timerWatch;
        private Capacities _tmpCapacities;
        
        [SerializeField] private CalibrationExercise _currentExercise;

        [BoxGroup("UI")] [SerializeField] private Text _dialogText;
        [BoxGroup("UI")] [SerializeField] private Text _exerciseCountText;
        [BoxGroup("UI")] [SerializeField] private Text _timerText;
        [BoxGroup("UI")] [SerializeField] private GameObject _enterButton;

        [SerializeField] private GameObject _clockObject;
        [SerializeField] private GameObject _dudeObject;

        private void Awake()
        {
            _serialController = FindObjectOfType<SerialController>();
            _serialController.OnSerialMessageReceived += OnSerialMessageReceived;
            _tmpCapacities = new Capacities();
            _flowWatch = new Stopwatch();
            _timerWatch = new Stopwatch();
            _capturedSamples = new Dictionary<long, float>();
            _calibrationLogger = new CalibrationLogger();

            _dudeObject.transform.Translate(-Camera.main.orthographicSize * Camera.main.aspect + (_dudeObject.transform.localScale.x / 2f), 0f, 0f);

            if (CalibrationToLoad > 0)
                _currentExercise = CalibrationToLoad;
        }

        private void Start()
        {
            DudeTalk("Vamos começar o exercício? Para avançar, pressione ENTER quando o ícone (►) aparecer!");
            StartCoroutine(ControlStates());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                NextStep();
        }

        private IEnumerator ControlStates()
        {
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
                                    DudeTalk("Você deve respirar somente pela boca. Não precisa morder o PITACO. Mantenha o PITACO sempre para baixo. Pressione (►) para continuar.");
                                    ReadyNextStep();
                                    break;

                                case 2:
                                    DudeTalk("Neste exercício, você deve RESPIRAR NORMALMENTE por 30 segundos. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                                    ReadyNextStep();
                                    break;

                                case 3:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    _capturedSamples.Clear();
                                    _serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    yield return new WaitForSeconds(1f);
                                    _dialogText.text = "(relaxe e RESPIRE NORMALMENTE)";

                                    AirFlowEnable();

                                    StartCoroutine(DisplayCountdown(32));
                                    while (_flowWatch.ElapsedMilliseconds < 32 * 1000)
                                        yield return null;

                                    AirFlowDisable();

                                    _flowMeter = FlowMath.MeanFlow(_capturedSamples);

                                    if (_flowMeter > RespiratoryFrequencyThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, _flowMeter);
                                        _tmpCapacities.RespCycleDuration = _flowMeter;
                                        ReadyNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, _flowMeter);
                                        DudeWarnUnknownFlow();
                                        ReadyStep(_currentStep);
                                        break;
                                    }

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Sua média de frequência respiratória é de {_tmpCapacities.RawRespCycleDuration / 1000f} seg/ciclo." +
                                             " Pressione (►) para continuar com os outros exercícios.");
                                    ReadyNextStep();
                                    break;

                                case 5:
                                    Pacient.Loaded.Capacities.RespCycleDuration = _tmpCapacities.RawRespCycleDuration;
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
                                    DudeTalk("Neste exercício, você deve INSPIRAR FORTE. Serão 3 tentativas. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                                    ReadyNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    _serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1f);

                                    AirFlowEnable();
                                    StartCoroutine(DisplayCountdown(10));
                                    _dialogText.text = "(INSPIRE FORTE e aguarde o próximo passo)";

                                    yield return new WaitForSeconds(10);

                                    AirFlowDisable();

                                    var insCheck = _flowMeter;
                                    ResetFlowMeter();

                                    if (insCheck < -Pacient.Loaded.PitacoThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, insCheck);
                                        _currentExerciseCount++;
                                        ReadyNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, insCheck);
                                        DudeWarnUnknownFlow();
                                        ReadyStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    ReadyStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu pico inspiratório é de {FlowMath.ToLitresPerMinute(_tmpCapacities.RawInsPeakFlow)} L/min." +
                                             " Pressione (►) para continuar com os outros exercícios.");
                                    ReadyNextStep();
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
                                    DudeTalk("Neste exercício, INSPIRE e MANTENHA o relógio GIRANDO o MÁXIMO QUE PUDER! Serão 3 tentativas. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                                    ReadyNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    _serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1f);

                                    AirFlowEnable(false);
                                    _dialogText.text = "(INSPIRE para manter o relógio GIRANDO o MÁXIMO QUE PUDER)";

                                    while (_flowMeter >= -Pacient.Loaded.PitacoThreshold)
                                        yield return null;

                                    _flowWatch.Restart();

                                    while (_flowMeter < -Pacient.Loaded.PitacoThreshold * 0.5f)
                                        yield return null;

                                    AirFlowDisable();
                                    
                                    ResetFlowMeter();

                                    // Check for player input
                                    if (_flowWatch.ElapsedMilliseconds > FlowTimeThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, _flowWatch.ElapsedMilliseconds);

                                        if (_flowWatch.ElapsedMilliseconds > _tmpCapacities.InsFlowDuration)
                                            _tmpCapacities.InsFlowDuration = _flowWatch.ElapsedMilliseconds;

                                        _currentExerciseCount++;

                                        ReadyNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, _flowWatch.ElapsedMilliseconds);
                                        DudeWarnUnknownFlow();
                                        ReadyStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    ReadyStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu tempo de inspiração é de {_tmpCapacities.RawInsFlowDuration / 1000f} segundos." +
                                             " Pressione (►) para continuar com os outros exercícios.");
                                    ReadyNextStep();
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
                                    DudeTalk("Neste exercício, você deve ASSOPRAR FORTE. Serão 3 tentativas. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                                    ReadyNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    _serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1f);

                                    AirFlowEnable();
                                    StartCoroutine(DisplayCountdown(10));
                                    _dialogText.text = "(ASSOPRE FORTE e aguarde o próximo passo)";

                                    // Wait for player input
                                    yield return new WaitForSeconds(10);

                                    AirFlowDisable();

                                    var expCheck = _flowMeter;
                                    ResetFlowMeter();

                                    if (expCheck > Pacient.Loaded.PitacoThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, expCheck);
                                        _currentExerciseCount++;
                                        ReadyNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, expCheck);
                                        DudeWarnUnknownFlow();
                                        ReadyStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    ReadyStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu pico expiratório é de {FlowMath.ToLitresPerMinute(_tmpCapacities.RawExpPeakFlow)} L/min." +
                                             " Pressione (►) para continuar com os outros exercícios.");
                                    ReadyNextStep();
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
                                    DudeTalk("Neste exercício, ASSOPRE e MANTENHA o relógio GIRANDO o MÁXIMO QUE PUDER! Serão 3 tentativas. Ao apertar (►), o relógio ficará verde para você começar o exercício.");
                                    ReadyNextStep();
                                    break;

                                case 2:
                                    if (!_serialController.IsConnected)
                                    {
                                        SerialDisconnectedWarning();
                                        continue;
                                    }

                                    _serialController.Recalibrate();
                                    _serialController.StartSampling();

                                    _exerciseCountText.text = $"Exercício: {_currentExerciseCount + 1}/3";
                                    yield return new WaitForSeconds(1f);

                                    AirFlowEnable(false);
                                    _dialogText.text = "(ASSOPRE e mantenha o relógio girando o MÁXIMO QUE PUDER)";

                                    // Wait for player input to be greather than threshold
                                    while (_flowMeter <= Pacient.Loaded.PitacoThreshold)
                                        yield return null;

                                    _flowWatch.Restart();

                                    while (_flowMeter > Pacient.Loaded.PitacoThreshold * 0.3f)
                                        yield return null;

                                    AirFlowDisable();
                                    
                                    ResetFlowMeter();

                                    // Check for player input
                                    if (_flowWatch.ElapsedMilliseconds > FlowTimeThreshold)
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Success, _currentExercise, _flowWatch.ElapsedMilliseconds);

                                        if (_flowWatch.ElapsedMilliseconds > _tmpCapacities.ExpFlowDuration)
                                            _tmpCapacities.ExpFlowDuration = _flowWatch.ElapsedMilliseconds;

                                        _currentExerciseCount++;
                                        ReadyNextStep(true);
                                        continue;
                                    }
                                    else
                                    {
                                        _calibrationLogger.Write(CalibrationExerciseResult.Failure, _currentExercise, _flowWatch.ElapsedMilliseconds);
                                        DudeWarnUnknownFlow();
                                        ReadyStep(_currentStep);
                                        break;
                                    }

                                case 3:
                                    DudeCongratulate();
                                    ReadyStep(_currentExerciseCount == 3 ? _currentStep + 1 : _currentStep - 1);
                                    break;

                                case 4:
                                    SoundManager.Instance.PlaySound("Success");
                                    DudeTalk($"Seu tempo de fluxo expiratório é de {_tmpCapacities.RawExpFlowDuration / 1000f} segundos." +
                                             " Pressione (►) para continuar com os outros exercícios.");
                                    ReadyNextStep();
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
            _clockObject.GetComponentInChildren<ClockArrow>().SpinClock = false;
            _acceptingValues = false;
        }

        private void AirFlowEnable(bool restartWatch = true)
        {
            if (restartWatch)
                _flowWatch.Restart();

            //FindObjectOfType<PitacoLogger>().Pause(false);

            _clockObject.GetComponent<SpriteRenderer>().color = Color.green;
            _clockObject.GetComponentInChildren<ClockArrow>().SpinClock = true;
            _acceptingValues = true;
        }

        private void SaveAndQuit()
        {
            Pacient.Loaded.CalibrationDone = Pacient.Loaded.IsCalibrationDone();
            PacientDb.Instance.Save();
            FindObjectOfType<PitacoLogger>().StopLogging();
            _calibrationLogger.Save();
            FindObjectOfType<SceneLoader>().LoadScene(0);
        }

        private void SerialDisconnectedWarning()
        {
            _enterButton.SetActive(true);
            DudeWarnPitacoDisconnected();
            ReadyStep(99);
        }
    }
}