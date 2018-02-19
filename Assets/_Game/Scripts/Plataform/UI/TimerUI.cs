using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField]
    private Text timerText;

    [SerializeField]
    private Image fillSprite;

    private StageManager stgMgr;

    private void Awake() => stgMgr = FindObjectOfType<StageManager>();

    private void FixedUpdate()
    {
        if (fillSprite.fillAmount > 1f)
            fillSprite.color = Color.cyan;

        fillSprite.fillAmount = stgMgr.Watch / Stage.Loaded.SpawnDuration;
        timerText.text = Mathf.Round(stgMgr.Watch).ToString(CultureInfo.InvariantCulture);
    }
}