using UnityEngine;
using UnityEngine.UI;

//ToDo - Feedback Sonoro
public class PanelMessage : MonoBehaviour
{
    public Image MessageIcon;
    public Text IconText;
    public Text MessageText;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void ShowPanel(string msg)
    {
        transform.localScale = _originalScale;
        MessageText.text = msg;
    }

    public void ShowError(string msg)
    {
        MessageIcon.color = Color.red;
        IconText.text = "X";
        IconText.color = Color.white;

        ShowPanel(msg);
        Debug.LogFormat("ShowError: {0}", msg);
    }

    public void ShowInfo(string msg)
    {
        MessageIcon.color = Color.white;
        IconText.text = "?";
        IconText.color = Color.black;

        ShowPanel(msg);
        Debug.LogFormat("ShowInfo: {0}", msg);
    }

    public void ShowWarning(string msg)
    {
        MessageIcon.color = Color.yellow;
        IconText.text = "!";
        IconText.color = Color.black;

        ShowPanel(msg);
        Debug.LogFormat("ShowWarning: {0}", msg);
    }

    public void PressOk()
    {
        transform.localScale = Vector3.zero;
    }
}
