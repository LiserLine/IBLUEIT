using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrationSceneManager : MonoBehaviour
{
    private bool _executeStep, _sceneOpen;
    private int _stepNum = 12; //ToDo - change back to 1 when code is done

    #region Flow Exercice Variables

    private float _flowMeter;
    private int _exerciseCounter;

    #endregion

    public Text firstTimeText, balloonText;
    public GameObject enterButton, enterButtonSmall, firstTimePanel, tutoDude, tutoClock, textBalloon;
    public SerialController serialController;
    public LevelLoader levelLoader;
    public ClockArrowSpin clockArrowSpin;

    private void Start()
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
        //ToDo - Translate const strings
        var firstTimeMsg = "Olá! Bem-vindo ao I Blue It!";
        var hereAgainMsg = "Olá! Vamos calibrar o PITACO novamente?";
        firstTimeText.text = GameManager.Instance.Player.CalibrationDone ? hereAgainMsg : firstTimeMsg;
        _stepNum = GameManager.Instance.Player.CalibrationDone ? 4 : _stepNum;
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
        GameManager.Instance.Player.CalibrationDone = false;
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
                    Destroy(firstTimePanel);
                    Destroy(enterButton);

                    // Enable dude and clock
                    tutoClock.SetActive(true);
                    tutoDude.SetActive(true);
                    textBalloon.SetActive(true);

                    // ToDo - should it be handled this way? I tried many other ways but no success
                    enterButton = enterButtonSmall;

                    _sceneOpen = true;
                }

                string dudeMsg;
                switch (_stepNum)
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
                        var expCheck = _flowMeter;
                        ResetFlowMeter();

                        if (expCheck > GameConstants.CalibrationThreshold) // ToDo - Check if 10 must be threshold to go to next step
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
                            dudeMsg = "Não consegui medir sua expiração. Vamos tentar novamente?";
                            DudeShowMessage(dudeMsg);
                            SetStep(5);
                            break;
                        }

                    case 6:
                        dudeMsg = "Muito bem!";
                        DudeShowMessage(dudeMsg);
                        //todo - quicky claps sounds
                        SetStep(_exerciseCounter == 3 ? 8 : 7);
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
                        var insCheck = _flowMeter;
                        ResetFlowMeter();

                        if (insCheck < -GameConstants.CalibrationThreshold)
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
                            dudeMsg = "Não consegui medir sua expiração. Vamos tentar novamente?";
                            DudeShowMessage(dudeMsg);
                            SetStep(9);
                            break;
                        }

                    case 10:
                        dudeMsg = "Muito bem!";
                        DudeShowMessage(dudeMsg);
                        //todo - quicky claps sounds
                        SetStep(_exerciseCounter == 3 ? 12 : 11);
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
                _executeStep = false; // Wait for player next command
            }

            yield return null;
        }
    }
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

    private void DudeShowMessage(string msg)
    {
        balloonText.text = msg;
        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
    }

    private void DudeClearMessage()
    {
        balloonText.text = "";
        tutoDude.GetComponent<Animator>().SetBool("Talking", false);
    }

    private void WarnPitacoDisconnected()
    {
        var dudeMsg = "O PITACO não está conectado. Conecte-o ao computador e reinicie o jogo!";
        DudeShowMessage(dudeMsg);
        SetStep(0);
    }

    private void OnSerialMessageReceived(string arrived)
    {
        if (arrived.Length > 1 && SerialGetOffset.IsUsingOffset)
        {
            var tmp = GameConstants.ParseSerialMessage(arrived);
            tmp -= SerialGetOffset.Offset;

            switch (_stepNum)
            {
                case 5: //expiratory peak
                    if (tmp > _flowMeter)
                    {
                        _flowMeter = tmp;

                        if (_flowMeter > GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow)
                        {
                            GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow = _flowMeter;
                            Debug.Log($"ExpiratoryPeakFlow: {GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow}");
                        }
                    }
                    break;

                case 9: //inspiratory peak
                    if (tmp < _flowMeter)
                    {
                        _flowMeter = tmp;

                        if (_flowMeter < GameManager.Instance.Player.RespiratoryInfo.InspiratoryPeakFlow)
                        {
                            GameManager.Instance.Player.RespiratoryInfo.InspiratoryPeakFlow = _flowMeter;
                            Debug.Log($"InspiratoryPeakFlow: {GameManager.Instance.Player.RespiratoryInfo.InspiratoryPeakFlow}");
                        }
                    }
                    break;
            }
        }
    }

    private void ResetExerciseCounter()
    {
        _exerciseCounter = 0;
    }

    private void ResetFlowMeter()
    {
        _flowMeter = 0f;
    }
}
