using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationSceneManager : MonoBehaviour
{
    public Text firstTimeText, balloonText;
    public GameObject enterButton, firstTimePanel, tutoDude, tutoClock, textBalloon;

    private bool waitingMessage;
    private int msgCount = 1;

    void Start()
    {
        firstTimeText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock00");
        enterButton.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!waitingMessage)
                StartCoroutine(ScreenFlow());
        }
    }

    IEnumerator ScreenFlow()
    {
        waitingMessage = true;
        firstTimeText.text = "";
        balloonText.text = "";
        enterButton.SetActive(false);
        tutoDude.GetComponent<Animator>().SetBool("Talking", false);

        yield return new WaitForSeconds(1);

        switch (msgCount)
        {
            case 1:
                firstTimeText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock01");
                enterButton.SetActive(true);
                break;
            case 2:
                firstTimeText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock02");
                enterButton.SetActive(true);
                break;
            case 3:
                firstTimePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                yield return new WaitForSeconds(1.5f);
                tutoClock.SetActive(true);
                tutoDude.SetActive(true);
                textBalloon.SetActive(true);
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock03");
                break;
            case 4:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock04");
                break;
            case 5:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock05");
                break;
            case 6:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock06");
                break;
            case 7:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock07");
                break;
            case 8:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock08");
                break;
            case 9:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock09");
                break;
            case 10:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock10");
                break;
            case 11:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock11");
                break;
            case 12:
                balloonText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock12");
                break;
            case 13:
                firstTimePanel.GetComponent<Image>().CrossFadeAlpha(255, 1, false);
                break;
            default:
                firstTimeText.text = "";
                balloonText.text = "";
                break;
        }

        tutoDude.GetComponent<Animator>().SetBool("Talking", true);

        msgCount++;
        waitingMessage = false;
    }
}
