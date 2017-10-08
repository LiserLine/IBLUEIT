using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationSceneManager : MonoBehaviour
{
    public Text firstTimeText, balloonText;
    public GameObject enterButton, firstTimePanel, tutoDude, tutoClock, textBalloon;
    public SerialController serialController;
    public LevelLoader levelLoader;
    public ClockArrowSpin clockArrowSpin;

    private bool triggerNextStep;
    private int stepNum = 1; //ToDo - change back to 1 when code is done

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
            TriggerNextStep();
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
            if (triggerNextStep)
            {
                // Clear screen
                firstTimeText.text = "";
                enterButton.SetActive(false);
                DudeStopTalking();

                // Wait 1 sec to show next step
                yield return new WaitForSeconds(1);

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
                        // Fade black screen and wait 1s 
                        firstTimePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                        yield return new WaitForSeconds(1);

                        // Enable dude and clock
                        tutoClock.SetActive(true);
                        tutoDude.SetActive(true);
                        textBalloon.SetActive(true);

                        dudeMsg = "Este é o relógio que vai medir a força e o tempo da sua respiração.";
                        DudeStartTalking(dudeMsg);

                        //ToDo - enter button new position and size
                        SetNextStep();
                        break;

                    case 4: // Tell player to do a Expiratory Peak Exercise
                        dudeMsg = "Quando o relógio ficar verde, inspire e assopre bem forte no PITACO!";
                        DudeStartTalking(dudeMsg);
                        SetNextStep();
                        break;

                    case 5:
                        if (serialController.IsConnected)
                        {
                            // Enable clock arrow spin and initialize pitaco value request
                            clockArrowSpin.SpinClock = true;
                            serialController.InitializePitacoRequest();
                            while (!SerialGetOffset.IsUsingOffset) yield return null;

                            // Change clock color to green so player can use pitaco
                            tutoClock.GetComponent<SpriteRenderer>().color = Color.green;
                            balloonText.text = "Inspire, assopre e aguarde.";

                            // Wait 8 seconds for player input
                            yield return new WaitForSeconds(8);

                            // Disable clock arrow spin and reset clock color
                            tutoClock.GetComponent<SpriteRenderer>().color = Color.white;
                            clockArrowSpin.SpinClock = false;

                            // Check for player input
                            // ToDo - Check if 10 must be threshold to go to next step
                            if (flowMeter < 10f)
                            {
                                dudeMsg = "Não consegui sentir sua respiração. Vamos tentar novamente?";
                                DudeStartTalking(dudeMsg);
                                GoToStep(4);
                                continue;
                            }

                            // If player passed threshold, go to next step
                            exercises++;
                            if (exercises == 2) GoToStep(7);
                            TriggerNextStep();
                        }
                        else // If PITACO is not connected
                        {
                            dudeMsg = "O PITACO não está conectado. Verifique sua conexão.";
                            DudeStartTalking(dudeMsg);
                            GoToStep(4);
                        }
                        break;

                    case 6:
                        dudeMsg = "Muito bem!";
                        DudeStartTalking(dudeMsg);
                        //todo - quicky claps sounds
                        GoToStep(exercises == 3 ? 8 : 7);
                        break;

                    case 7:
                        dudeMsg = "Mais uma vez!";
                        DudeStartTalking(dudeMsg);
                        GoToStep(5);
                        break;

                    #endregion

                    #region Inspiration Peak

                    case 8:
                        dudeMsg = "Agora, inspire com força!";
                        DudeStartTalking(dudeMsg);
                        SetNextStep();
                        break;

                    case 9:



                        break;
                        
                        #endregion

                        #region Expiration Time
                        #endregion

                        #region Inspiration Time
                        #endregion

                        #region Flow Measurement
                        #endregion


                        //case 13:
                        //    levelLoader.LoadScene(2); tutodone = true;
                        //    break;
                }

                enterButton.SetActive(true); // Enable enter button sprite everytime
                triggerNextStep = false;
            }

            yield return null;
        }
    }

    public void TriggerNextStep()
    {
        triggerNextStep = true;
    }

    void GoToStep(int stepNum)
    {
        this.stepNum = stepNum;
    }

    void SetNextStep()
    {
        this.stepNum++;
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
                    }
                    break;
            }
        }
    }
}
