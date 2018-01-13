using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private Text timerText;

    [SerializeField]
    private Image fillSprite;

    private void FixedUpdate()
    {
        if (StageManager.Instance.Timer / StageManager.Instance.PlaySessionTime > 1f)
            return;

        fillSprite.fillAmount = StageManager.Instance.Timer / StageManager.Instance.PlaySessionTime;
        timerText.text = Mathf.Round(StageManager.Instance.Timer).ToString();
    }
}
