using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private Text timerText;

    [SerializeField]
    private Image fillSprite;

    private void Awake() => StageManager.Instance.OnStageTimeUp += PlayTimeUpAudio;

    private void PlayTimeUpAudio() => AudioManager.Instance.PlaySound("PlataformTimeUp");

    private void FixedUpdate()
    {
        if (StageManager.Instance.Timer / StageManager.Instance.PlaySessionTime > 1f)
            fillSprite.color = Color.cyan;

        fillSprite.fillAmount = StageManager.Instance.Timer / StageManager.Instance.PlaySessionTime;
        timerText.text = Mathf.Round(StageManager.Instance.Timer).ToString();
    }
}
