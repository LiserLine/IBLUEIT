using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationCanvasManager : MonoBehaviour
{
    public Text blackScreenText, balloonText;
    public GameObject enterButton, firstTimePanel;

    private bool waitingMessage;
    private int msgCount = 1;

    void Start()
    {
        blackScreenText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock00");
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
        blackScreenText.text = "";
        enterButton.SetActive(false);

        yield return new WaitForSeconds(1);

        switch (msgCount)
        {
            case 1:
                blackScreenText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock01");
                break;
            case 2:
                blackScreenText.text = LocalizationManager.Instance.GetLocalizedValue("tutorialClock02");
                break;
            case 3:
                firstTimePanel.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                yield return new WaitForSeconds(1);

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
            default:
                balloonText.text = "";
                break;
        }

        enterButton.SetActive(true);
        msgCount++;
        waitingMessage = false;
    }
}
