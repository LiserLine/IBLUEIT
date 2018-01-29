using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using NaughtyAttributes;

public enum CalibrationSteps
{
    RespiratoryFrequency,
    InspiratoryPeak,
    InspiratoryFlow,
    ExpiratoryPeak,
    ExpiratoryFlow
}

//ToDo - Implement StateMachine ?
//ToDo - UI should inherit BasicUI

public class CalibrationController : MonoBehaviour
{
    [BoxGroup("UI")]
    [SerializeField]
    private Text welcomeText, balloonText, stepCountValue;

    [BoxGroup("UI")]
    [SerializeField]
    private GameObject enterButton, enterButtonSmall, welcomePanel, textBalloon;

    [BoxGroup("Calibration")]
    [SerializeField]
    private GameObject dude, clock;

    [BoxGroup("Calibration")]
    [SerializeField]
    private ClockArrow clockArrow;

    private bool executeStep, infoPanelOpen, acceptingValues;
    private int stepCount = 1;
    private int exerciseCount;
    private const int flowTimeThreshold = 1000; // In Miliseconds
    private const int respiratoryFrequencyThreshold = 700; //In Milliseconds //ToDo - Test this variable before implementing in CSV
    private float tempFlowMeter;
    private RespiratoryInfo tempRespiratoryInfo;
    private Stopwatch watch;
    private Dictionary<long, float> samples;
    private CalibrationSteps stepRunning;
    private bool calibrationDone;

    private void Start()
    {
        tempRespiratoryInfo = new RespiratoryInfo();
        watch = new Stopwatch();
        samples = new Dictionary<long, float>();
        welcomeText.text = "Primeiro, precisamos calibrar a sua respiração. Vamos lá?";
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
        while (!calibrationDone)
        {
            if (executeStep)
            {
                // Clear screen
                welcomeText.text = "";
                enterButton.SetActive(false);
                DudeClearMessage();

                // Wait 1 sec to show next step
                yield return new WaitForSeconds(1);

                // Open Scene Animation
                // Opens on step 3 or greater
                if (stepCount >= 2 && !infoPanelOpen)
                {
                    // Fade black screen and wait 1s 
                    welcomePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                    yield return new WaitForSeconds(1);
                    welcomePanel.SetActive(false);
                    Destroy(enterButton);

                    // Enable dude and clock
                    clock.SetActive(true);
                    dude.SetActive(true);
                    textBalloon.SetActive(true);

                    enterButton = enterButtonSmall; // Should it be handled this way? I tried to scale in many other ways but no success

                    infoPanelOpen = true;
                }
                
                switch (stepCount)
                {
                    //#region Inviting steps

                    case 1:
                        welcomeText.text = "Neste jogo, você deve respirar somente pela boca. Não precisa morder o PITACO.";
                        SetNextStep();
                        break;

                    //#endregion

                    #region Respiratory Frequency

                    case 2:
                        SerialController.Instance.Recalibrate();
                        ResetExerciseCounter();
                        ResetFlowMeter();
                        stepRunning = CalibrationSteps.RespiratoryFrequency;
                        DudeShowMessage("Este relógio medirá sua respiração. Aguarde ele ficar verde e respire tranquilamente no PITACO por 30 segundos.");
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
                        watch.Restart();

                        while (watch.ElapsedMilliseconds < 30 * 1000)
                            yield return null;

                        watch.Stop();

                        tempFlowMeter = Utils.CalculateMeanFlow(samples.ToList());

                        Debug.Log($"RespirationFrequency: {tempFlowMeter}");

                        DisableClockFlow();

                        if (tempFlowMeter > respiratoryFrequencyThreshold)
                        {
                            tempRespiratoryInfo.RespirationFrequency = tempFlowMeter;
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
                        DudeShowMessage("Muito bem! Agora, vamos continuar com os outros exercícios.");
                        SetNextStep();
                        break;

                    #endregion

                    #region Inspiration Peak

                    case 5:
                        SerialController.Instance.Recalibrate();
                        PrepareNextExercise();
                        stepRunning = CalibrationSteps.InspiratoryPeak;
                        DudeShowMessage("Agora mediremos sua força. Quando o relógio ficar verde, INSPIRE bem forte.");
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

                        var insCheck = tempFlowMeter;
                        ResetFlowMeter();

                        if (insCheck < -GameMaster.PitacoThreshold)
                        {
                            exerciseCount++;

                            if (exerciseCount == 2)
                                SetStep(stepCount + 2, true);
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
                        stepRunning = CalibrationSteps.ExpiratoryPeak;
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
                        var expCheck = tempFlowMeter;
                        ResetFlowMeter();

                        if (expCheck > GameMaster.PitacoThreshold)
                        {
                            exerciseCount++;

                            if (exerciseCount == 2)
                                SetStep(stepCount + 2, true);
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
                        stepRunning = CalibrationSteps.ExpiratoryFlow;
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

                        watch.Reset();

                        // Wait for player input to be greather than threshold
                        while (tempFlowMeter <= GameMaster.PitacoThreshold)
                            yield return null;

                        watch.Start();

                        while (tempFlowMeter > GameMaster.PitacoThreshold * 0.6f)
                            yield return null;

                        DisableClockFlow();

                        watch.Stop();

                        Debug.Log($"Expiration Time: {watch.ElapsedMilliseconds} ms ({watch.ElapsedMilliseconds / 1000} secs)");

                        // Check for player input
                        ResetFlowMeter();
                        if (watch.ElapsedMilliseconds > flowTimeThreshold)
                        {
                            if (watch.ElapsedMilliseconds > tempRespiratoryInfo.ExpiratoryFlowTime)
                                tempRespiratoryInfo.ExpiratoryFlowTime = watch.ElapsedMilliseconds;

                            exerciseCount++;

                            if (exerciseCount == 2)
                                SetStep(stepCount + 2, true);
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
                        stepRunning = CalibrationSteps.InspiratoryFlow;
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
                        watch.Reset();

                        while (tempFlowMeter >= -GameMaster.PitacoThreshold)
                            yield return null;

                        watch.Start();

                        while (tempFlowMeter < -GameMaster.PitacoThreshold * 0.25f)  // Inspirating is weaker than expirating
                            yield return null;

                        watch.Stop();

                        DisableClockFlow();

                        Debug.Log($"Inspiration Time: {watch.ElapsedMilliseconds} ms ({watch.ElapsedMilliseconds / 1000} secs)");

                        // Check for player input
                        ResetFlowMeter();
                        if (watch.ElapsedMilliseconds > flowTimeThreshold)
                        {
                            if (watch.ElapsedMilliseconds > tempRespiratoryInfo.InspiratoryFlowTime)
                                tempRespiratoryInfo.InspiratoryFlowTime = watch.ElapsedMilliseconds;

                            exerciseCount++;

                            if (exerciseCount == 2)
                                SetStep(stepCount + 2, true);
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
                        calibrationDone = true;
                        PlayerData.Player.RespiratoryInfo = tempRespiratoryInfo;
                        PlayerDb.Instance.Save();

                        clock.SetActive(false);
                        dude.SetActive(false);
                        textBalloon.SetActive(false);

                        welcomePanel.SetActive(true); //ToDo - try to crossfade alpha from 0 to max

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
        stepCount = step;
        executeStep = jumpToStep;
    }

    /// <summary>
    /// Sets the next step to be executed on next step iteration.
    /// </summary>
    /// <param name="jumpToStep">Flag to execute the step automatically</param>
    private void SetNextStep(bool jumpToStep = false)
    {
        stepCount++;
        executeStep = jumpToStep;
    }

    private void DudeShowMessage(string msg)
    {
        balloonText.text = msg;
        dude.GetComponent<Animator>().SetBool("Talking", true);
    }

    private void DudeCongratulate()
    {
        var dudeMsg = "Muito bem!";
        DudeShowMessage(dudeMsg);
        AudioManager.Instance.PlaySound("Success");
        SetStep(exerciseCount == 3 ? stepCount + 2 : stepCount + 1);

        if (SerialController.Instance.IsConnected)
            SerialController.Instance.Recalibrate();
    }

    private void DudeAskAgain()
    {
        var dudeMsg = "Mais uma vez!";
        DudeShowMessage(dudeMsg);
        SetStep(stepCount - 2);

        if (SerialController.Instance.IsConnected)
            SerialController.Instance.Recalibrate();
    }

    private void DudeWarnUnknownFlow()
    {
        var dudeMsg = "Não consegui medir sua respiração. Vamos tentar novamente?";
        DudeShowMessage(dudeMsg);
        AudioManager.Instance.PlaySound("Failure");
        SetStep(stepCount);
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
        dude.GetComponent<Animator>().SetBool("Talking", false);
    }
    
    private void EnableClockFlow()
    {
        clock.GetComponent<SpriteRenderer>().color = Color.green;
        clockArrow.SpinClock = true;
        acceptingValues = true;
    }

    private void DisableClockFlow()
    {
        clock.GetComponent<SpriteRenderer>().color = Color.white;
        clockArrow.SpinClock = false;
        acceptingValues = false;
    }

    private void ResetExerciseCounter() => exerciseCount = 0;

    private void ResetFlowMeter() => tempFlowMeter = 0f;

    private void PrepareNextExercise()
    {
        ResetExerciseCounter();
        ResetFlowMeter();

        if (SerialController.Instance.IsConnected)
            SerialController.Instance.Recalibrate();
    }

    private void OnSerialMessageReceived(string arrived)
    {
        if (!acceptingValues || arrived.Length < 1)
            return;

        var tmp = Utils.ParseFloat(arrived);

        switch (stepRunning)
        {
            case CalibrationSteps.ExpiratoryPeak:
                if (tmp > tempFlowMeter)
                {
                    tempFlowMeter = tmp;

                    if (tempFlowMeter > tempRespiratoryInfo.ExpiratoryPeakFlow)
                    {
                        tempRespiratoryInfo.ExpiratoryPeakFlow = tempFlowMeter;
                        Debug.Log($"ExpiratoryPeakFlow: {tempFlowMeter}");
                    }
                }
                break;

            case CalibrationSteps.InspiratoryPeak:
                if (tmp < tempFlowMeter)
                {
                    tempFlowMeter = tmp;

                    if (tempFlowMeter < tempRespiratoryInfo.InspiratoryPeakFlow)
                    {
                        tempRespiratoryInfo.InspiratoryPeakFlow = tempFlowMeter;
                        Debug.Log($"InspiratoryPeakFlow: {tempFlowMeter}");
                    }
                }
                break;

            case CalibrationSteps.ExpiratoryFlow:
            case CalibrationSteps.InspiratoryFlow:
                tempFlowMeter = tmp;
                break;

            case CalibrationSteps.RespiratoryFrequency:
                if (watch.IsRunning)
                    samples.Add(watch.ElapsedMilliseconds, tmp);
                break;
        }
    }
}
