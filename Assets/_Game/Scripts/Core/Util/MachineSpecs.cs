using System;
using UnityEngine;

public class MachineSpecs : MonoBehaviour
{
    public static string Get() => "System Information:\n\n" +
                                           $"- Machine Name: {Environment.MachineName}\n" +
                                           $"- User Name: {Environment.UserName}\n" +
                                           $"- OS Version: {Environment.OSVersion}\n" +
                                           $"- Is64BitOperatingSystem: {Environment.Is64BitOperatingSystem}";
}