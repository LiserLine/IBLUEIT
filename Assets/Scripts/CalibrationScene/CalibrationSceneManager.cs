using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationSceneManager : MonoBehaviour
{
    public Text firstTimeText, balloonText;
    public GameObject enterButton, enterButtonSmall, firstTimePanel, tutoDude, tutoClock, textBalloon;
    public SerialController serialController;
    public LevelLoader levelLoader;
    public ClockArrowSpin clockArrowSpin;

    private bool executeStep, sceneOpen;
    private int stepNum = 5; //ToDo - change back to 1 when code is done

    void Start()
    {
        firstTimeText.text = "Olá! Bem-vindo ao I Blue It!";
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
        var tmp_tutorialDone = false; //Todo - plrTutorialDone

        while (!tmp_tutorialDone)
        {
            if (executeStep)
            {
                // Clear screen
                firstTimeText.text = "";
                enterButton.SetActive(false);
                DudeStopTalking();

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
                        DudeStartTalking(dudeMsg);
                        SetNextStep();
                        break;

                    case 4: // Tell player to do a Expiratory Peak Exercise
                        dudeMsg = "Quando o relógio ficar verde, inspire e assopre bem forte no PITACO. Serão três exercícios!";
                        DudeStartTalking(dudeMsg);
                        SetNextStep();
                        break;

                    case 5:
                        if (!serialController.IsConnected)
                        {
                            dudeMsg = "O PITACO não está conectado. Conecte-o ao computador e reinicie o jogo!";
                            DudeStartTalking(dudeMsg);
                            SetStep(0);
                            continue; // Avoid break and execute the setted step
                        }

                        // Enable clock arrow spin and initialize pitaco value request
                        clockArrowSpin.SpinClock = true;
                        serialController.InitializePitacoRequest();
                        while (!SerialGetOffset.IsUsingOffset) yield return null;

                        // Change clock color to green so player can use pitaco
                        tutoClock.GetComponent<SpriteRenderer>().color = Color.green;
                        balloonText.text = "Inspire, assopre bem forte e aguarde.";

                        // Wait 8 seconds for player input
                        yield return new WaitForSeconds(8);

                        // Disable clock arrow spin and reset clock color
                        tutoClock.GetComponent<SpriteRenderer>().color = Color.white;
                        clockArrowSpin.SpinClock = false;

                        // Check for player input
                        var flowCheck = flowMeter;
                        ResetFlowVariables();

                        if (flowCheck > 10f) // ToDo - Check if 10 must be threshold to go to next step
                        {
                            exercises++;

                            if (exercises == 2)
                                SetStep(7, true);
                            else
                                SetNextStep(true);
                            
                            continue;
                        }
                        else
                        {
                            dudeMsg = "Não consegui medir sua expiração. Vamos tentar novamente?";
                            DudeStartTalking(dudeMsg);
                            SetStep(5);
                            break;
                        }

                    case 6:
                        dudeMsg = "Muito bem!";
                        DudeStartTalking(dudeMsg);
                        //todo - quicky claps sounds
                        SetStep(exercises == 3 ? 8 : 7);
                        break;

                    case 7:
                        dudeMsg = "Mais uma vez!";
                        DudeStartTalking(dudeMsg);
                        SetStep(5);
                        break;

                    #endregion

                    #region Inspiration Peak

                    case 8:
                        dudeMsg = "Agora, inspire com força!";
                        DudeStartTalking(dudeMsg);
                        SetNextStep();
                        break;

                    #endregion

                    #region Expiration Time
                    #endregion

                    #region Inspiration Time
                    #endregion

                    #region Flow Measurement
                    #endregion


                    //case ??:
                    //    levelLoader.LoadScene(2); tutodone = true;
                    //    break;

                    default:
                        var activeScene = SceneManager.GetActiveScene().buildIndex;
                        levelLoader.LoadScene(activeScene);
                        break;
                }

                enterButton.SetActive(true); // Enable enter button sprite everytime
                executeStep = false; // Wait for player next command
            }

            yield return null;
        }
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

    void DudeStartTalking(string msg)
    {
        balloonText.text = msg;
        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
    }

    void DudeStopTalking()
    {
        balloonText.text = "";
        tutoDude.GetComponent<Animator>().SetBool("Talking", false);
    }

    //ToDo code tag compiled unity editor
    //ToDo better coding
    private float flowRecord;
    private float flowMeter;
    private int exercises;
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

                        Debug.Log($"ExpiratoryPeakFlow: {flowMeter}");

                        if (flowMeter > flowRecord)
                        {
                            flowRecord = flowMeter;
                            Debug.Log($"New Expiratory Record: {flowRecord}");
                        }
                    }
                    break;
            }
        }
    }

    void ResetFlowVariables()
    {
        flowMeter = 0f;
    }
}
