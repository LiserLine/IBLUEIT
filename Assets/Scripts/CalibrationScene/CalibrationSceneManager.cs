﻿// ToDo - Translate strings

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class CalibrationSceneManager : MonoBehaviour
{
    private bool _executeStep, _sceneOpen, _firstTimePlaying;
    private int _stepNum = 16; //ToDo - change back to 1 when code is done
    private int _exerciseCounter;
    private const int FlowTimeThreshold = 1000; // In Miliseconds
    private float _flowMeter;
    private RespiratoryInfo _respiratoryInfoTemp;
    private Stopwatch _stopwatch;
    private Dictionary<long, float> _respiratoryFrequencyDictionary;

    public Text firstTimeText, balloonText;
    public GameObject enterButton, enterButtonSmall, firstTimePanel, tutoDude, tutoClock, textBalloon;
    public SerialController serialController;
    public LevelLoader levelLoader;
    public ClockArrowSpin clockArrowSpin;

    private void Awake()
    {

#if UNITY_EDITOR
        if (GameManager.Instance.Player == null)
        {
            GameManager.Instance.Player = new Player
            {
                Name = "NetRunner",
                Id = 0,
                CalibrationDone = false,
            };
        }
#endif

        // Initialization
        _respiratoryInfoTemp = new RespiratoryInfo();
        _stopwatch = new Stopwatch();
        _respiratoryFrequencyDictionary = new Dictionary<long, float>();

        _firstTimePlaying = !GameManager.Instance.Player.CalibrationDone;
        GameManager.Instance.Player.CalibrationDone = false;

        var firstTimeMsg = "Olá! Bem-vindo ao I Blue It!";
        var hereAgainMsg = "Precisando calibrar o PITACO? Então vamos lá!";
        firstTimeText.text = _firstTimePlaying ? firstTimeMsg : hereAgainMsg;
        _stepNum = _firstTimePlaying ? _stepNum : 4;

        enterButton.SetActive(true);
        serialController.OnSerialMessageReceived += OnSerialMessageReceived;
        StartCoroutine(ScreenSteps());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteNextStep();
        }
    }

    private void OnDisable()
    {
        serialController.OnSerialMessageReceived -= OnSerialMessageReceived;
    }

    private IEnumerator ScreenSteps()
    {
        while (!GameManager.Instance.Player.CalibrationDone)
        {
            if (_executeStep)
            {
                // Clear screen
                firstTimeText.text = "";
                enterButton.SetActive(false);
                DudeClearMessage();

                // Wait 1 sec to show next step
                yield return new WaitForSeconds(1);

                // Open Scene Animation
                // Opens on step 3 or greater
                if (_stepNum >= 3 && !_sceneOpen)
                {
                    // Fade black screen and wait 1s 
                    firstTimePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                    yield return new WaitForSeconds(1);
                    firstTimePanel.SetActive(false);
                    Destroy(enterButton);

                    // Enable dude and clock
                    tutoClock.SetActive(true);
                    tutoDude.SetActive(true);
                    textBalloon.SetActive(true);

                    enterButton = enterButtonSmall; // Should it be handled this way? I tried to scale in many other ways but no success

                    _sceneOpen = true;
                }

                string dudeMsg;
                switch (_stepNum)
                {
                    #region Inviting steps

                    case 1:
                        firstTimeText.text = "Primeiro, vamos calibrar a sua respiração.";
                        SetNextStep();
                        break;

                    case 2:
                        firstTimeText.text = "Então, vamos lá?";
                        SetNextStep();
                        break;

                    #endregion

                    #region Expiration Peak

                    case 3:
                        dudeMsg = "Este relógio medirá a força e o tempo da sua respiração. Para isso, faremos alguns exercícios.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 4:
                        dudeMsg = "Primeiro, vamos medir sua força. Quando o relógio ficar verde, inspire e assopre bem forte no PITACO!";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 5:
                        if (!serialController.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;

                        balloonText.text = "(inspire, assopre bem forte no PITACO e aguarde)";

                        EnableClockFlow();

                        // Wait 8 seconds for player input
                        yield return new WaitForSeconds(8);

                        DisableClockFlow();

                        // Check for player input
                        var expCheck = _flowMeter;
                        ResetFlowMeter();

                        if (expCheck > GameConstants.PitacoThreshold)
                        {
                            _exerciseCounter++;

                            if (_exerciseCounter == 2)
                                SetStep(7, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 6:
                        DudeCongratulate();
                        break;

                    case 7:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Inspiration Peak

                    case 8:
                        ResetExerciseAndFlowMeter();
                        dudeMsg = "Agora faremos o mesmo para inspiração. Quando o relógio ficar verde, expire e então inspire bem forte.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 9:
                        if (!serialController.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;
#endif
                        yield return new WaitForSeconds(1);
                        balloonText.text = "(expire, inspire com força e aguarde)";
                        EnableClockFlow();
                        yield return new WaitForSeconds(8);
                        DisableClockFlow();

                        var insCheck = _flowMeter;
                        ResetFlowMeter();

                        if (insCheck < -GameConstants.PitacoThreshold)
                        {
                            _exerciseCounter++;

                            if (_exerciseCounter == 2)
                                SetStep(11, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 10:
                        DudeCongratulate();
                        break;

                    case 11:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Expiration Time

                    case 12:
                        ResetExerciseAndFlowMeter();
                        dudeMsg = "Agora vamos medir o tempo de expiração. Relaxe e expire o máximo de tempo possível no PITACO.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 13:
                        if (!serialController.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;
#endif

                        yield return new WaitForSeconds(1);
                        balloonText.text = "(relaxe, expire o máximo de tempo possível e aguarde)";
                        EnableClockFlow();

                        // Wait for player input to be greather than threshold
                        _stopwatch.Reset();

                        while (_flowMeter <= GameConstants.PitacoThreshold)
                            yield return null;

                        _stopwatch.Start();

                        while (_flowMeter > GameConstants.PitacoThreshold)
                            yield return null;

                        DisableClockFlow();

                        _stopwatch.Stop();

                        Debug.Log($"Expiration Time: {_stopwatch.ElapsedMilliseconds} ms ({_stopwatch.ElapsedMilliseconds / 1000} secs)");

                        // Check for player input
                        ResetFlowMeter();
                        if (_stopwatch.ElapsedMilliseconds > FlowTimeThreshold)
                        {
                            if (_stopwatch.ElapsedMilliseconds > _respiratoryInfoTemp.ExpiratoryFlowTime)
                                _respiratoryInfoTemp.ExpiratoryFlowTime = _stopwatch.ElapsedMilliseconds;

                            _exerciseCounter++;

                            if (_exerciseCounter == 2)
                                SetStep(15, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 14:
                        DudeCongratulate();
                        break;

                    case 15:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Inspiration Time

                    case 16:
                        ResetExerciseAndFlowMeter();
                        dudeMsg = "Agora vamos medir o tempo de inspiração. Relaxe e inspire o máximo de tempo possível no PITACO.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 17:
                        if (!serialController.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;
#endif

                        yield return new WaitForSeconds(1);
                        balloonText.text = "(relaxe, inspire o máximo de tempo possível e aguarde)";
                        EnableClockFlow();

                        // Wait for player input to be greather than threshold
                        _stopwatch.Reset();

                        while (_flowMeter >= -GameConstants.PitacoThreshold)
                            yield return null;

                        _stopwatch.Start();

                        while (_flowMeter < -GameConstants.PitacoThreshold)
                            yield return null;

                        _stopwatch.Stop();

                        DisableClockFlow();

                        Debug.Log($"Inspiration Time: {_stopwatch.ElapsedMilliseconds} ms ({_stopwatch.ElapsedMilliseconds / 1000} secs)");

                        // Check for player input
                        ResetFlowMeter();
                        if (_stopwatch.ElapsedMilliseconds > FlowTimeThreshold)
                        {
                            if (_stopwatch.ElapsedMilliseconds > _respiratoryInfoTemp.ExpiratoryFlowTime)
                                _respiratoryInfoTemp.ExpiratoryFlowTime = _stopwatch.ElapsedMilliseconds;

                            _exerciseCounter++;

                            if (_exerciseCounter == 2)
                                SetStep(19, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 18:
                        DudeCongratulate();
                        break;

                    case 19:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Flow Measurement

                    case 20:
                        ResetExerciseCounter();
                        ResetFlowMeter();
                        dudeMsg = "E para finalizar, respire tranquilamente no PITACO por 60 segundos.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 21:
                        if (!serialController.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;
#endif

                        yield return new WaitForSeconds(1);
                        balloonText.text = "(relaxe e respire usando o PITACO por 1 minuto.)";
                        EnableClockFlow();

                        // Wait for player input to be greather than threshold
                        _stopwatch.Restart();

                        while (_stopwatch.ElapsedMilliseconds < 65000)
                            yield return null;

                        _stopwatch.Stop();

                        CalculateMeanFlow();

                        DisableClockFlow();

                        if (_flowMeter > 0) // ToDo
                        {
                            if (_stopwatch.ElapsedMilliseconds > _respiratoryInfoTemp.ExpiratoryFlowTime)
                                _respiratoryInfoTemp.ExpiratoryFlowTime = _stopwatch.ElapsedMilliseconds;

                            SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 22:
                        DudeCongratulate();
                        break;

                    #endregion

                    #region Ending Steps

                    case 24:
                        dudeMsg = "Ótimo, você está pronto para começar a jogar! Bom jogo!";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 25:
                        GameManager.Instance.Player.CalibrationDone = true;
                        GameManager.Instance.Player.RespiratoryInfo = _respiratoryInfoTemp;
                        DatabaseManager.Instance.Players.Save();
                        firstTimePanel.SetActive(false);
                        firstTimePanel.GetComponent<Image>().CrossFadeAlpha(1, 1, false);
                        yield return new WaitForSeconds(1.5f);
                        levelLoader.LoadScene(2);
                        break;

                    #endregion

                    default: // Reload Scene
                        levelLoader.LoadScene(3);
                        break;
                }

                enterButton.SetActive(true); // Enable enter button sprite on break
                _executeStep = false; // Wait for player next command
            }

            yield return null;
        }
    }

    private void CalculateMeanFlow()
    {
        // ToDo - try lambda expressions
        bool semiCicleCompleted = false;
        bool cicleCompleted = false;
        long firstValueInstant = 0;
        long lastValueInstant = 0;
        long timeSum = 0;
        int timeCount = 0;
        float v1 = 0f;

        var pairs = _respiratoryFrequencyDictionary.ToList();
        for (var i = 0; i < pairs.Count; i++)
        {
            var key = pairs[i].Key;
            var value = pairs[i].Value;

            if (value < -GameConstants.PitacoThreshold && value > GameConstants.PitacoThreshold)
            {
                if (firstValueInstant == 0)
                {
                    firstValueInstant = pairs[i - 1].Key;
                    v1 = value;
                }

                if (semiCicleCompleted && v1 >= value)
                {
                    semiCicleCompleted = false;
                    cicleCompleted = false;
                    firstValueInstant = 0;
                    lastValueInstant = 0;
                    timeSum = 0;
                    timeCount = 0;
                    v1 = 0f;
                }

                if (firstValueInstant > 0)
                {
                    timeSum += key;
                    timeCount++;
                }
            }
            else
            {
                if (semiCicleCompleted)
                {
                    cicleCompleted = true;

                }

                if (firstValueInstant > 0)
                {
                    semiCicleCompleted = true;
                }
            }
        }
    }

    #region Step Controllers

    public void ExecuteNextStep()
    {
        _executeStep = true;
    }

    private void SetStep(int step, bool jumpToStep = false)
    {
        _stepNum = step;
        _executeStep = jumpToStep;
    }

    private void SetNextStep(bool jumpToStep = false)
    {
        _stepNum++;
        _executeStep = jumpToStep;
    }

    #endregion

    #region Dude Messages

    private void DudeShowMessage(string msg)
    {
        balloonText.text = msg;
        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
    }

    private void DudeCongratulate()
    {
        var dudeMsg = "Muito bem!";
        DudeShowMessage(dudeMsg);
        //todo - quicky claps sounds
        SetStep(_exerciseCounter == 3 ? _stepNum + 2 : _stepNum + 1);
    }

    private void DudeAskAgain()
    {
        var dudeMsg = "Mais uma vez!";
        DudeShowMessage(dudeMsg);
        SetStep(_stepNum - 2);
    }

    private void DudeWarnUnknownFlow()
    {
        var dudeMsg = "Não consegui medir sua respiração. Vamos tentar novamente?";
        DudeShowMessage(dudeMsg);
        SetStep(_stepNum);
    }

    private void DudeWarnPitacoDisconnected()
    {
        var dudeMsg = "O PITACO não está conectado. Conecte-o ao computador!";
        enterButton.SetActive(true);
        DudeShowMessage(dudeMsg);
        SetStep(0);
    }

    private void DudeClearMessage()
    {
        balloonText.text = "";
        tutoDude.GetComponent<Animator>().SetBool("Talking", false);
    }

    #endregion

    #region TutoClock Controller

    private void EnableClockFlow()
    {
        tutoClock.GetComponent<SpriteRenderer>().color = Color.green;
        clockArrowSpin.SpinClock = true;
    }

    private void DisableClockFlow()
    {
        tutoClock.GetComponent<SpriteRenderer>().color = Color.white;
        clockArrowSpin.SpinClock = false;
    }

    #endregion

    #region Resetters

    private void ResetExerciseCounter()
    {
        _exerciseCounter = 0;
    }

    private void ResetFlowMeter()
    {
        _flowMeter = 0f;
    }

    private void ResetExerciseAndFlowMeter()
    {
        ResetExerciseCounter();
        ResetFlowMeter();
    }

    #endregion

    private void OnSerialMessageReceived(string arrived)
    {
        if (arrived.Length < 1 || !SerialGetOffset.IsUsingOffset)
            return;

        var tmp = GameConstants.ParseSerialMessage(arrived);
        tmp -= SerialGetOffset.Offset;

        switch (_stepNum)
        {
            case 5: // expiratory peak
                if (tmp > _flowMeter)
                {
                    _flowMeter = tmp;

                    if (_flowMeter > _respiratoryInfoTemp.ExpiratoryPeakFlow)
                    {
                        _respiratoryInfoTemp.ExpiratoryPeakFlow = _flowMeter;
                        Debug.Log($"ExpiratoryPeakFlow: {_respiratoryInfoTemp.ExpiratoryPeakFlow}");
                    }
                }
                break;

            case 9: // inspiratory peak
                if (tmp < _flowMeter)
                {
                    _flowMeter = tmp;

                    if (_flowMeter < _respiratoryInfoTemp.InspiratoryPeakFlow)
                    {
                        _respiratoryInfoTemp.InspiratoryPeakFlow = _flowMeter;
                        Debug.Log($"InspiratoryPeakFlow: {_respiratoryInfoTemp.InspiratoryPeakFlow}");
                    }
                }
                break;

            case 13: // expiratory flow time
            case 17: // inspiratory flow time
                _flowMeter = tmp;
                break;

            case 21: // respiratory frequency
                if (_stopwatch.IsRunning)
                    _respiratoryFrequencyDictionary.Add(_stopwatch.ElapsedMilliseconds, tmp);
                break;
        }
    }

}