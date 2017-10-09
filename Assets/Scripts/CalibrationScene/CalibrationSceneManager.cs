using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationSceneManager : MonoBehaviour
{
    private bool executeStep, sceneOpen;
    private int stepNum = 1; //ToDo - change back to 1 when code is done

    #region Flow Exercice Variables

    private float flowMeter;
    private int exerciseCounter;

    #endregion

    public Text firstTimeText, balloonText;
    public GameObject enterButton, enterButtonSmall, firstTimePanel, tutoDude, tutoClock, textBalloon;
    public SerialController serialController;
    public LevelLoader levelLoader;
    public ClockArrowSpin clockArrowSpin;

    void Start()
    {

#if UNITY_EDITOR
        if (GameManager.Instance.Player == null)
        {
            GameManager.Instance.Player = new Player
            {
                Name = "NetRunner",
                Id = 0
            };
        }
#endif

        var firstTimeMsg = "Olá! Bem-vindo ao I Blue It!";
        var hereAgainMsg = "Olá! Vamos calibrar o PITACO novamente?";
        firstTimeText.text = GameManager.Instance.Player.CalibrationDone ? hereAgainMsg : firstTimeMsg;
        enterButton.SetActive(true);
        serialController.OnSerialMessageReceived += OnSerialMessageReceived;
        StartCoroutine(ScreenSteps());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteNextStep();
        }
    }

    void OnDisable()
    {
        serialController.OnSerialMessageReceived -= OnSerialMessageReceived;
    }

    IEnumerator ScreenSteps()
    {
        GameManager.Instance.Player.CalibrationDone = false;
        while (!GameManager.Instance.Player.CalibrationDone)
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
                if (stepNum >= 3 && !sceneOpen)
                {
                    // Fade black screen and wait 1s 
                    firstTimePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                    yield return new WaitForSeconds(1);
                    Destroy(firstTimePanel);
                    Destroy(enterButton);

                    // Enable dude and clock
                    tutoClock.SetActive(true);
                    tutoDude.SetActive(true);
                    textBalloon.SetActive(true);

                    // ToDo - should it be handled this way? I tried many other ways but no success
                    enterButton = enterButtonSmall;

                    sceneOpen = true;
                }

                string dudeMsg;
                switch (stepNum)
                {
                    #region Inviting steps

                    case 1:
                        // Must use the firstTimeText up to case 2
                        firstTimeText.text = "Primeiro, vamos calibrar a sua respiração.";
                        SetNextStep();
                        break;

                    case 2:
                        firstTimeText.text = "Então, vamos começar?";
                        SetNextStep();
                        break;

                    #endregion

                    #region Expiration Peak

                    case 3:
                        dudeMsg = "Este é o relógio que vai medir a força e o tempo da sua respiração.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 4: // Tell player to do a Expiratory Peak Exercise
                        dudeMsg = "Quando o relógio ficar verde, inspire e assopre bem forte no PITACO. Faremos exercícios!";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 5:
                        if (!serialController.IsConnected)
                        {
                            WarnPitacoDisconnected();
                            continue;
                        }

                        // Enable clock arrow spin and initialize pitaco value request
                        clockArrowSpin.SpinClock = true;
                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;

                        // Change clock color to green so player can use pitaco
                        tutoClock.GetComponent<SpriteRenderer>().color = Color.green;
                        balloonText.text = "Inspire, assopre bem forte no PITACO e aguarde.";

                        // Wait 8 seconds for player input
                        yield return new WaitForSeconds(8);

                        // Disable clock arrow spin and reset clock color
                        tutoClock.GetComponent<SpriteRenderer>().color = Color.white;
                        clockArrowSpin.SpinClock = false;

                        // Check for player input
                        var expCheck = flowMeter;
                        ResetFlowMeter();

                        if (expCheck > GameConstants.CalibrationThreshold) // ToDo - Check if 10 must be threshold to go to next step
                        {
                            exerciseCounter++;

                            if (exerciseCounter == 2)
                                SetStep(7, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            dudeMsg = "Não consegui medir sua expiração. Vamos tentar novamente?";
                            DudeShowMessage(dudeMsg);
                            SetStep(5);
                            break;
                        }

                    case 6:
                        dudeMsg = "Muito bem!";
                        DudeShowMessage(dudeMsg);
                        //todo - quicky claps sounds
                        SetStep(exerciseCounter == 3 ? 8 : 7);
                        break;

                    case 7:
                        dudeMsg = "Mais uma vez!";
                        DudeShowMessage(dudeMsg);
                        SetStep(5);
                        break;

                    #endregion

                    #region Inspiration Peak

                    case 8:
                        ResetExerciseCounter();
                        ResetFlowMeter();
                        dudeMsg = "Agora faremos o mesmo exercício para inspiração. Expire, e então inspire bem forte no PITACO.";
                        DudeShowMessage(dudeMsg);
                        SetNextStep();
                        break;

                    case 9:
                        if (!serialController.IsConnected)
                        {
                            WarnPitacoDisconnected();
                            continue;
                        }

                        // Enable clock arrow spin
                        clockArrowSpin.SpinClock = true;

#if UNITY_EDITOR
                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;
#endif

                        // Change clock color to green so player can use pitaco
                        tutoClock.GetComponent<SpriteRenderer>().color = Color.green;
                        balloonText.text = "Expire, inspire bem forte no PITACO e aguarde.";

                        // Wait 8 seconds for player input
                        yield return new WaitForSeconds(8);

                        // Disable clock arrow spin and reset clock color
                        tutoClock.GetComponent<SpriteRenderer>().color = Color.white;
                        clockArrowSpin.SpinClock = false;

                        // Check for player input
                        var insCheck = flowMeter;
                        ResetFlowMeter();

                        if (insCheck < -GameConstants.CalibrationThreshold)
                        {
                            exerciseCounter++;

                            if (exerciseCounter == 2)
                                SetStep(11, true);
                            else
                                SetNextStep(true);

                            continue;
                        }
                        else
                        {
                            dudeMsg = "Não consegui medir sua expiração. Vamos tentar novamente?";
                            DudeShowMessage(dudeMsg);
                            SetStep(9);
                            break;
                        }

                    case 10:
                        dudeMsg = "Muito bem!";
                        DudeShowMessage(dudeMsg);
                        //todo - quicky claps sounds
                        SetStep(exerciseCounter == 3 ? 12 : 11);
                        break;

                    case 11:
                        dudeMsg = "Mais uma vez!";
                        DudeShowMessage(dudeMsg);
                        SetStep(9);
                        break;

                    #endregion

                    #region Expiration Time

                    case 12:
                        throw new NotImplementedException();
                        break;

                    #endregion

                    #region Inspiration Time
                    #endregion

                    #region Flow Measurement
                    #endregion

                    case 999: //ToDo - change this later
                        levelLoader.LoadScene(2);
                        GameManager.Instance.Player.CalibrationDone = true;
                        break;

                    default:
                        var activeScene = SceneManager.GetActiveScene().buildIndex;
                        levelLoader.LoadScene(activeScene);
                        break;
                }

                enterButton.SetActive(true); // Enable enter button sprite on break
                executeStep = false; // Wait for player next command
            }

            yield return null;
        }
    }

    private void WarnPitacoDisconnected()
    {
        var dudeMsg = "O PITACO não está conectado. Conecte-o ao computador e reinicie o jogo!";
        DudeShowMessage(dudeMsg);
        SetStep(0);
    }

    public void ExecuteNextStep()
    {
        executeStep = true;
    }

    void SetStep(int stepNum, bool jumpToNextStep = false)
    {
        this.stepNum = stepNum;
        executeStep = jumpToNextStep;
    }

    void SetNextStep(bool jumpToNextStep = false)
    {
        stepNum++;
        executeStep = jumpToNextStep;
    }

    void DudeShowMessage(string msg)
    {
        balloonText.text = msg;
        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
    }

    void DudeClearMessage()
    {
        balloonText.text = "";
        tutoDude.GetComponent<Animator>().SetBool("Talking", false);
    }

    void OnSerialMessageReceived(string arrived)
    {
        if (arrived.Length > 1 && SerialGetOffset.IsUsingOffset)
        {
            var tmp = GameConstants.ParseSerialMessage(arrived);
            tmp -= SerialGetOffset.Offset;

            switch (stepNum)
            {
                case 5: //expiratory peak
                    if (tmp > flowMeter)
                    {
                        flowMeter = tmp;

                        if (flowMeter > GameManager.Instance.Player.ExpiratoryPeakFlow)
                        {
                            GameManager.Instance.Player.ExpiratoryPeakFlow = flowMeter;
                            Debug.Log($"New Expiratory Peak Record: {GameManager.Instance.Player.ExpiratoryPeakFlow}");
                        }
                    }
                    break;

                case 9: //inspiratory peak
                    if (tmp < flowMeter)
                    {
                        flowMeter = tmp;

                        if (flowMeter < GameManager.Instance.Player.InspiratoryPeakFlow)
                        {
                            GameManager.Instance.Player.InspiratoryPeakFlow = flowMeter;
                            Debug.Log($"New Inspiratory Peak Record: {GameManager.Instance.Player.InspiratoryPeakFlow}");
                        }
                    }
                    break;
            }
        }
    }

    void ResetExerciseCounter()
    {
        exerciseCounter = 0;
    }

    void ResetFlowMeter()
    {
        flowMeter = 0f;
    }
}
