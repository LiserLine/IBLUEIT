using UnityEngine;

public class DebugOnScreen : MonoBehaviour
{
    private string _messageOutput;

    private void OnEnable()
    {
        Application.logMessageReceived += (message, stacktrace, type) =>
        {
            _messageOutput = message;
        };
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 20, Screen.width / 2f, 50f), _messageOutput);
    }
}
