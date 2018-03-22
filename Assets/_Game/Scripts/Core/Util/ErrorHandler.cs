using Ibit.Core.Game;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Ibit.Core.Util
{
    public class ErrorHandler : MonoBehaviour
    {
#if !UNITY_EDITOR
    private void OnEnable() => Application.logMessageReceived += OnLogMessageReceived;
    private void OnDisable() => Application.logMessageReceived -= OnLogMessageReceived;
#endif

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (!this.isActiveAndEnabled)
                return;

            if (type != LogType.Exception || type == LogType.Error)
                return;

            var errContent = $"{message}\r\n\r\n{stackTrace}\r\n\r\n{MachineSpecs.Get()}";

            SysMessage.Error("The game will be closed because of an unexpected program error. Please report this error to the developer.\n\n" +
                             "An error file has been created!.\n\n" + errContent, "[Error Handler] An unexpected error has occured!!!");

            FileReader.WriteAllText(@"DUMP/" + $"crash_{DateTime.Now:yyyyMMdd-HHmmss}.txt", errContent);

            FindObjectOfType<GameManager>().QuitGame();
        }

        [Button]
        private void TestError()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            throw new Exception("This is a test");
        }
    }
}