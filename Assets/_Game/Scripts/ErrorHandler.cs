using System;
using UnityEngine;

public class ErrorHandler : MonoBehaviour
{
    private bool hasError;
    private string errorMsg, stack;

    private Rect windowRect, labelRect;

    private void Awake()
    {
        windowRect = new Rect(0, 0, Screen.width, Screen.height);
        labelRect = new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.7f);
        Application.logMessageReceived += OnLogMessageReceived;
    }

    private void OnLogMessageReceived(string message, string stackTrace, LogType type)
    {
        if (type != LogType.Exception || type == LogType.Error)
            return;

        errorMsg = message;
        stack = stackTrace;

        Time.timeScale = 0f;

        hasError = true;
    }

    private void OnGUI()
    {
        if (!hasError)
            return;

        GUI.color = Color.red;

        GUI.Box(windowRect, ""); GUI.Box(windowRect, ""); GUI.Box(windowRect, ""); //opacity hax

        windowRect = GUI.Window(0, windowRect, Flush, "[Error Handler] Unexpected program error occured!!!");

        GUI.Label(labelRect,
            "The game will be close because of an unexpected program error.\n" +
            "Please report this error to the developer.\n" +
            $"A screenshot has been generated at \"{Environment.CurrentDirectory}\".\n" +
            $"Time Stamp: {DateTime.Now:s}\n\n" +
            $"{errorMsg}\n\n{stack}");
    }

    private void Flush(int windowID)
    {
        if (GUI.Button(new Rect(Screen.width * 0.45f, Screen.height * 0.9f, 100, 30), "OK"))
        {
            ScreenCapture.CaptureScreenshot($"ibit_error_{DateTime.Now:yyyyMMdd-HHmmss}.png");
            Application.Quit();
        }
    }
}
