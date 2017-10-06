using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationSceneManager : MonoBehaviour
{
    public Text firstTimeText, balloonText;
    public GameObject enterButton, miniEnterButton, firstTimePanel, tutoDude, tutoClock, textBalloon;
    public SerialController serialController;
    public LevelLoader levelLoader;
    public ClockArrowSpin clockArrowSpin;

    private bool triggerNextStep;
    private int stepCount; //ToDo - change back to 1 when code is done

    void Start()
    {
        firstTimeText.text = "Olá! Bem-vindo ao I Blue It!";
        stepCount++;
        enterButton.SetActive(true);
        serialController.OnSerialMessageReceived += OnSerialMessageReceived;
        StartCoroutine(ScreenSteps());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            triggerNextStep = true;
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
                firstTimeText.text = "";
                balloonText.text = "";
                enterButton.SetActive(false);
                miniEnterButton.SetActive(false);
                tutoDude.GetComponent<Animator>().SetBool("Talking", false);

                yield return new WaitForSeconds(1);

                switch (stepCount)
                {
                    #region Inviting steps

                    case 1:
                        firstTimeText.text = "Primeiro, vamos calibrar a sua respiração.";
                        enterButton.SetActive(true);
                        stepCount++;
                        break;

                    case 2:
                        firstTimeText.text = "Então, vamos começar?";
                        enterButton.SetActive(true);
                        stepCount++;
                        break;

                    #endregion

                    #region Expiration Peak

                    case 3:
                        firstTimePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                        yield return new WaitForSeconds(1.5f);
                        tutoClock.SetActive(true);
                        tutoDude.SetActive(true);
                        textBalloon.SetActive(true);
                        balloonText.text = "Este é o relógio que vai medir a força e o tempo da sua respiração.";
                        miniEnterButton.SetActive(true);
                        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        stepCount++;
                        break;

                    case 4:
                        balloonText.text = "Quando o relógio ficar verde, inspire e assopre bem forte no PITACO!";
                        miniEnterButton.SetActive(true);
                        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        stepCount++;
                        break;

                    case 5:
                        if (serialController.IsConnected)
                        {
                            clockArrowSpin.SpinClock = true;
                            serialController.InitializePitacoRequest();
                            while (!SerialGetOffset.IsUsingOffset) yield return null;

                            tutoClock.GetComponent<SpriteRenderer>().color = Color.green;
                            balloonText.text = "Inspire, assopre e aguarde.";
                            yield return new WaitForSeconds(8f);
                            tutoClock.GetComponent<SpriteRenderer>().color = Color.white;

                            if (flowMeter > 10f)
                            {
                                exercises++;
                                if (exercises == 2) stepCount = 7;
                                triggerNextStep = true;
                                continue;
                            }

                            tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                            balloonText.text = "Não consegui sentir sua respiração. Vamos tentar novamente?";
                            stepCount = 4;
                        }
                        else
                        {
                            balloonText.text = "O PITACO não está conectado. Verifique sua conexão.";
                            stepCount = 4;
                        }
                        break;

                    case 6:
                        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        balloonText.text = "Muito bem!";
                        //todo - aplausos
                        stepCount = exercises == 3 ? 8 : 7;
                        break;

                    case 7:
                        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        balloonText.text = "Mais uma vez!";
                        stepCount = 5;
                        break;

                    #endregion


                    #region Inspiration Peak

                    case 8:
                        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        balloonText.text = "Agora, inspire com força!";
                        break;


                        #endregion


                        //case 13:
                        //    levelLoader.LoadScene(2); tutodone = true;
                        //    break;
                }


                triggerNextStep = false;
            }

            yield return null;
        }
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

            switch (stepCount)
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
