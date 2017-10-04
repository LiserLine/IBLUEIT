using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationSceneManager : MonoBehaviour
{
    public Text firstTimeText, balloonText;
    public GameObject enterButton, miniEnterButton, firstTimePanel, tutoDude, tutoClock, textBalloon;
    public SerialListener serialListener;
    public LevelLoader levelLoader;

    private bool triggerNextStep;
    private int msgCount = 3; //ToDo - change back to 1 when code is done

    void Start()
    {
        firstTimeText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock00");
        enterButton.SetActive(true);
        serialListener.OnSerialMessageReceived += WaitForFlow;
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
        serialListener.OnSerialMessageReceived -= WaitForFlow;
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

                switch (msgCount)
                {
                    case 1:
                        firstTimeText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock01");
                        enterButton.SetActive(true);
                        msgCount++;
                        break;
                    case 2:
                        firstTimeText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock02");
                        enterButton.SetActive(true);
                        msgCount++;
                        break;
                    case 3:
                        firstTimePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                        yield return new WaitForSeconds(1.5f);
                        tutoClock.SetActive(true);
                        tutoDude.SetActive(true);
                        textBalloon.SetActive(true);
                        balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock03");
                        miniEnterButton.SetActive(true);
                        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        msgCount++;
                        break;
                    case 4:
                        balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock04");
                        miniEnterButton.SetActive(true);
                        tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        msgCount++;
                        break;
                    case 5:
                        serialListener.InitValueRequest();
                        //balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock05");
                        //tutoDude.GetComponent<Animator>().SetBool("Talking", true);
                        while (!SerialGetOffset.IsUsingOffset)
                        {
                            yield return null;
                        }

                        yield return new WaitForSeconds(5f);

                        if (expFlow > 100)
                        {
                            msgCount++;
                            triggerNextStep = true;
                            continue;
                        }
                        else
                        {

                        }
                        break;
                    case 6:
                        balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock06");
                        break;
                        //case 7:
                        //    balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock07");
                        //    break;
                        //case 8:
                        //    balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock08");
                        //    break;
                        //case 9:
                        //    balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock09");
                        //    break;
                        //case 10:
                        //    balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock10");
                        //    break;
                        //case 11:
                        //    balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock11");
                        //    break;
                        //case 12:
                        //    balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock12");
                        //    break;
                        //case 13:
                        //    levelLoader.LoadScene(2); tutodone
                        //    break;
                }


                triggerNextStep = false;
            }

            yield return null;
        }
    }

    private float expFlow;
    void WaitForFlow(string arrivedMsg)
    {
        //ToDo code tag compiled

        if (arrivedMsg.Length > 1 && SerialGetOffset.IsUsingOffset)
        {
            var tmp = GameConstants.ParseSerialMessage(arrivedMsg);
            tmp -= SerialGetOffset.Offset;
            switch (msgCount)
            {
                case 5: //expiratory peak
                    if (tmp > expFlow)
                    {
                        expFlow = tmp;
                        Debug.Log($"ExpiratoryPeakFlow: {expFlow}");
                    }
                    break;
            }
        }
    }
}
