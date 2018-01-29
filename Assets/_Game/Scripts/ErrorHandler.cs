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
        labelRect = new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.95f, Screen.height * 0.95f);
    }

#if UNITY_EDITOR
#else
    private void OnEnable() => Application.logMessageReceived += OnLogMessageReceived;
    private void OnDisable() => Application.logMessageReceived -= OnLogMessageReceived;
#endif
    
    private void OnLogMessageReceived(string message, string stackTrace, LogType type)
    {
        if (hasError || !this.isActiveAndEnabled)
            return;

        if (type != LogType.Exception || type == LogType.Error)
            return;

        errorMsg = message;
        stack = stackTrace;

        Time.timeScale = 0f;

        hasError = true;

        SysMessage.Error("The game will be closed because of an unexpected program error. Please report this error to the developer.\n\n" +
                         $"A screenshot will be saved at\"{Environment.CurrentDirectory}\".\n\n{errorMsg}\n\n{stack}",
            "[Error Handler] An unexpected error has occured!!!");

        Dump();
    }

    private void OnGUI()
    {
        if (!hasError)
            return;

        GUI.Box(windowRect, ""); GUI.Box(windowRect, ""); GUI.Box(windowRect, ""); //opacity hax

        windowRect = GUI.Window(0, windowRect, WindowFunction, errorMsg);

        GUI.Label(labelRect, $"{Utils.MachineSpecs()}\n\nStacktrace:\n\n{stack}");
    }

    private void WindowFunction(int windowId)
    { }

    private static void Dump()
    {
        ScreenCapture.CaptureScreenshot($"ibit_error_{DateTime.Now:yyyyMMdd-HHmmss}.png");
        Application.Quit();
    }
}
