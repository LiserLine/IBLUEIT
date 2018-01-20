using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public enum CalibrationSteps
{
    RespiratoryFrequency,
    InspiratoryPeak,
    InspiratoryFlow,
    ExpiratoryPeak,
    ExpiratoryFlow
}

//ToDo - Implement StateMachine
//ToDo - UI should inherit BasicUI

public class CalibrationController : MonoBehaviour
{
    [SerializeField]
    private Text firstTimeText, balloonText;

    [SerializeField]
    private GameObject enterButton, enterButtonSmall, firstTimePanel, tutoDude, tutoClock, textBalloon;

    [SerializeField]
    private ClockArrowSpin clockArrowSpin;

    private bool executeStep, sceneOpen, firstTimePlaying, acceptingValues;
    private int stepNum = 2;
    private int exerciseCounter;
    private const int flowTimeThreshold = 1000; // In Miliseconds
    private const int respiratoryFrequencyThreshold = 700; //In Milliseconds //ToDo - Test this variable before implementing in CSV
    private float flowMeter;
    private RespiratoryInfo respiratoryInfoTemp;
    private Stopwatch stopwatch;
    private Dictionary<long, float> respiratorySamples;
    private CalibrationSteps runningStep;

    private void Start()
    {
        respiratoryInfoTemp = new RespiratoryInfo();
        stopwatch = new Stopwatch();
        respiratorySamples = new Dictionary<long, float>();
        firstTimePlaying = !PlayerData.Player.CalibrationDone;
        firstTimeText.text = "Primeiro, precisamos calibrar a sua respiração. Vamos lá?";
        enterButton.SetActive(true);
        SerialController.Instance.OnSerialMessageReceived += OnSerialMessageReceived;
        StartCoroutine(ScreenSteps());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteNextStep();
        }
    }

    private IEnumerator ScreenSteps()
    {
        while (!PlayerData.Player.CalibrationDone)
        {
            if (executeStep)
            {
                // Clear screen
                firstTimeText.text = "";
                enterButton.SetActive(false);
                DudeClearMessage();

                // Wait 1 sec to show next step
                yield return new WaitForSeconds(1);

                // Open Scene Animation
                // Opens on step 3 or greater
                if (stepNum >= 2 && !sceneOpen)
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

                    sceneOpen = true;
                }

                string dudeMsg;
                switch (stepNum)
                {
                    //#region Inviting steps

                    //case 1:
                    //    firstTimeText.text = "Primeiro, precisamos calibrar a sua respiração. Vamos lá?";
                    //    SetNextStep();
                    //    break;

                    //#endregion

                    #region Respiratory Frequency

                    case 2:
                        SerialController.Instance.Recalibrate();
                        ResetExerciseCounter();
                        ResetFlowMeter();
                        runningStep = CalibrationSteps.RespiratoryFrequency;
                        dudeMsg = "Este relógio medirá sua respiração. Quando ficar verde, respire normal e tranquilamente no PITACO por 30 segundos.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 3:
                        if (!SerialController.Instance.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        SerialController.Instance.InitSampling();
#endif

                        yield return new WaitForSeconds(1);
                        balloonText.text = "(relaxe e respire por 30 segundos)";
                        EnableClockFlow();

                        // Wait for player input to be greather than threshold
                        stopwatch.Restart();

                        while (stopwatch.ElapsedMilliseconds < 30 * 1000)
                            yield return null;

                        stopwatch.Stop();

                        flowMeter = Utils.CalculateMeanFlow(respiratorySamples.ToList());

                        Debug.Log($"RespirationFrequency: {flowMeter}");

                        DisableClockFlow();

                        if (flowMeter > respiratoryFrequencyThreshold)
                        {
                            respiratoryInfoTemp.RespirationFrequency = flowMeter;
                            SetNextStep(true);
                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 4:
                        AudioManager.Instance.PlaySound("Success");
                        dudeMsg = "Muito bem! Agora, vamos continuar com os outros exercícios.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    #endregion

                    #region Inspiration Peak

                    case 5:
                        SerialController.Instance.Recalibrate();
                        PrepareNextExercise();
                        runningStep = CalibrationSteps.InspiratoryPeak;
                        dudeMsg = "Agora mediremos sua força. Quando o relógio ficar verde, INSPIRE bem forte.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 6:
                        if (!SerialController.Instance.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        SerialController.Instance.InitSampling();
#endif
                        yield return new WaitForSeconds(1);
                        balloonText.text = "(inspire com força e aguarde)";
                        EnableClockFlow();
                        yield return new WaitForSeconds(7);
                        DisableClockFlow();

                        var insCheck = flowMeter;
                        ResetFlowMeter();

                        if (insCheck < -GameMaster.PitacoThreshold)
                        {
                            exerciseCounter++;

                            if (exerciseCounter == 2)
                                SetStep(stepNum + 2, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 7:

                        DudeCongratulate();
                        break;

                    case 8:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Expiration Peak

                    case 9:
                        SerialController.Instance.Recalibrate();
                        dudeMsg = "Agora faremos ao contrário. Quando o relógio ficar verde, ASSOPRE bem forte.";
                        PrepareNextExercise();
                        runningStep = CalibrationSteps.ExpiratoryPeak;
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 10:
                        if (!SerialController.Instance.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        SerialController.Instance.InitSampling();
#endif

                        balloonText.text = "(assopre bem forte no PITACO e aguarde)";

                        EnableClockFlow();

                        // Wait 8 seconds for player input
                        yield return new WaitForSeconds(7);

                        DisableClockFlow();

                        // Check for player input
                        var expCheck = flowMeter;
                        ResetFlowMeter();

                        if (expCheck > GameMaster.PitacoThreshold)
                        {
                            exerciseCounter++;

                            if (exerciseCounter == 2)
                                SetStep(stepNum + 2, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 11:
                        DudeCongratulate();
                        break;

                    case 12:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Expiration Time

                    case 13:
                        SerialController.Instance.Recalibrate();
                        PrepareNextExercise();
                        runningStep = CalibrationSteps.ExpiratoryFlow;
                        dudeMsg = "Agora vamos medir o tempo. Quando o relógio ficar verde, relaxe e ASSOPRE o máximo de tempo possível.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 14:
                        if (!SerialController.Instance.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        SerialController.Instance.InitSampling();
#endif

                        yield return new WaitForSeconds(1);
                        balloonText.text = "(expire o máximo de tempo possível e aguarde)";
                        EnableClockFlow();

                        stopwatch.Reset();

                        // Wait for player input to be greather than threshold
                        while (flowMeter <= GameMaster.PitacoThreshold)
                            yield return null;

                        stopwatch.Start();

                        while (flowMeter > GameMaster.PitacoThreshold * 0.6f)
                            yield return null;

                        DisableClockFlow();

                        stopwatch.Stop();

                        Debug.Log($"Expiration Time: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedMilliseconds / 1000} secs)");

                        // Check for player input
                        ResetFlowMeter();
                        if (stopwatch.ElapsedMilliseconds > flowTimeThreshold)
                        {
                            if (stopwatch.ElapsedMilliseconds > respiratoryInfoTemp.ExpiratoryFlowTime)
                                respiratoryInfoTemp.ExpiratoryFlowTime = stopwatch.ElapsedMilliseconds;

                            exerciseCounter++;

                            if (exerciseCounter == 2)
                                SetStep(stepNum + 2, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 15:
                        DudeCongratulate();
                        break;

                    case 16:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Inspiration Time

                    case 17:
                        SerialController.Instance.Recalibrate();
                        PrepareNextExercise();
                        runningStep = CalibrationSteps.InspiratoryFlow;
                        dudeMsg = "Agora, quando o relógio ficar verde, INSPIRE o máximo de tempo possível.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 18:
                        if (!SerialController.Instance.IsConnected)
                        {
                            DudeWarnPitacoDisconnected();
                            continue;
                        }

#if UNITY_EDITOR
                        SerialController.Instance.InitSampling();
#endif

                        yield return new WaitForSeconds(1);
                        balloonText.text = "(inspire o máximo de tempo possível e aguarde)";
                        EnableClockFlow();

                        // Wait for player input to be greather than threshold
                        stopwatch.Reset();

                        while (flowMeter >= -GameMaster.PitacoThreshold)
                            yield return null;

                        stopwatch.Start();

                        while (flowMeter < -GameMaster.PitacoThreshold * 0.25f)  // Inspirating is weaker than expirating
                            yield return null;

                        stopwatch.Stop();

                        DisableClockFlow();

                        Debug.Log($"Inspiration Time: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedMilliseconds / 1000} secs)");

                        // Check for player input
                        ResetFlowMeter();
                        if (stopwatch.ElapsedMilliseconds > flowTimeThreshold)
                        {
                            if (stopwatch.ElapsedMilliseconds > respiratoryInfoTemp.InspiratoryFlowTime)
                                respiratoryInfoTemp.InspiratoryFlowTime = stopwatch.ElapsedMilliseconds;

                            exerciseCounter++;

                            if (exerciseCounter == 2)
                                SetStep(stepNum + 2, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            DudeWarnUnknownFlow();
                            break;
                        }

                    case 19:
                        DudeCongratulate();
                        break;

                    case 20:
                        DudeAskAgain();
                        break;

                    #endregion

                    #region Ending Steps

                    case 21:
                        AudioManager.Instance.PlaySound("Claps");
                        dudeMsg = "Ótimo, agora você está pronto para começar a jogar! Bom jogo!";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 22:
                        PlayerData.Player.CalibrationDone = true;
                        PlayerData.Player.RespiratoryInfo = respiratoryInfoTemp;
                        PlayerDb.Instance.Save();

                        tutoClock.SetActive(false);
                        tutoDude.SetActive(false);
                        textBalloon.SetActive(false);

                        firstTimePanel.SetActive(true); //ToDo - try to crossfade alpha from 0 to max

                        SceneLoader.Instance.LoadScene(0); //ToDo - ternario para quando implementar minigames
                        break;

                    #endregion

                    case 99: // Reload Scene
                        SceneLoader.Instance.LoadScene(2);
                        break;
                }

                enterButton.SetActive(true); // Enable enter button sprite on break
                executeStep = false; // Wait for player next command
            }

            yield return null;
        }
    }

    #region Step Controllers

    /// <summary>
    /// Method to execute next step of calibration.
    /// Some buttons use this to execute the next step.
    /// </summary>
    public void ExecuteNextStep() => executeStep = true;

    /// <summary>
    /// Sets a step to be executed on next step iteration.
    /// </summary>
    /// <param name="step">Step number</param>
    /// <param name="jumpToStep">Flag to execute the step automatically</param>
    private void SetStep(int step, bool jumpToStep = false)
    {
        stepNum = step;
        executeStep = jumpToStep;
    }

    /// <summary>
    /// Sets the next step to be executed on next step iteration.
    /// </summary>
    /// <param name="jumpToStep">Flag to execute the step automatically</param>
    private void SetNextStep(bool jumpToStep = false)
    {
        stepNum++;
        executeStep = jumpToStep;
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
        AudioManager.Instance.PlaySound("Success");
        SetStep(exerciseCounter == 3 ? stepNum + 2 : stepNum + 1);
    }

    private void DudeAskAgain()
    {
        var dudeMsg = "Mais uma vez!";
        DudeShowMessage(dudeMsg);
        SetStep(stepNum - 2);
    }

    private void DudeWarnUnknownFlow()
    {
        var dudeMsg = "Não consegui medir sua respiração. Vamos tentar novamente?";
        DudeShowMessage(dudeMsg);
        AudioManager.Instance.PlaySound("Failure");
        SetStep(stepNum);
    }

    private void DudeWarnPitacoDisconnected()
    {
        var dudeMsg = "O PITACO não está conectado. Conecte-o ao computador!";
        enterButton.SetActive(true);
        DudeShowMessage(dudeMsg);
        SetStep(99);
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
        acceptingValues = true;
    }

    private void DisableClockFlow()
    {
        tutoClock.GetComponent<SpriteRenderer>().color = Color.white;
        clockArrowSpin.SpinClock = false;
        acceptingValues = false;
    }

    #endregion

    #region Resetters

    private void ResetExerciseCounter()
    {
        exerciseCounter = 0;
    }

    private void ResetFlowMeter()
    {
        flowMeter = 0f;
    }

    private void PrepareNextExercise()
    {
        ResetExerciseCounter();
        ResetFlowMeter();

        if (SerialController.Instance.IsConnected)
            SerialController.Instance.Recalibrate();
    }

    #endregion

    private void OnSerialMessageReceived(string arrived)
    {
        if (!acceptingValues || arrived.Length < 1)
            return;

        var tmp = Utils.ParseFloat(arrived);

        switch (runningStep)
        {
            case CalibrationSteps.ExpiratoryPeak:
                if (tmp > flowMeter)
                {
                    flowMeter = tmp;

                    if (flowMeter > respiratoryInfoTemp.ExpiratoryPeakFlow)
                    {
                        respiratoryInfoTemp.ExpiratoryPeakFlow = flowMeter;
                        Debug.Log($"ExpiratoryPeakFlow: {flowMeter}");
                    }
                }
                break;

            case CalibrationSteps.InspiratoryPeak:
                if (tmp < flowMeter)
                {
                    flowMeter = tmp;

                    if (flowMeter < respiratoryInfoTemp.InspiratoryPeakFlow)
                    {
                        respiratoryInfoTemp.InspiratoryPeakFlow = flowMeter;
                        Debug.Log($"InspiratoryPeakFlow: {flowMeter}");
                    }
                }
                break;

            case CalibrationSteps.ExpiratoryFlow:
            case CalibrationSteps.InspiratoryFlow:
                flowMeter = tmp;
                break;

            case CalibrationSteps.RespiratoryFrequency:
                if (stopwatch.IsRunning)
                    respiratorySamples.Add(stopwatch.ElapsedMilliseconds, tmp);
                break;
        }
    }
}
