using UnityEngine;
using UnityEngine.UI;

//ToDo - Feedback Sonoro
public class PanelMessage : MonoBehaviour
{
    public Image MessageIcon;
    public Text IconText;
    public Text MessageText;

    private void ShowPanel(string msg)
    {
        this.gameObject.SetActive(true);
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

    public void ShowMessage(string msg)
    {
        MessageIcon.color = Color.white;
        IconText.text = "?";
        IconText.color = Color.black;

        ShowPanel(msg);
        Debug.LogFormat("ShowMessage: {0}", msg);
    }

    public void ShowWarning(string msg)
    {
        MessageIcon.color = Color.yellow;
        IconText.text = "!";
        IconText.color = Color.black;

        ShowPanel(msg);
        Debug.LogFormat("ShowWarning: {0}", msg);
    }

    public void PressOK()
    {
        this.gameObject.SetActive(false);
    }
}
