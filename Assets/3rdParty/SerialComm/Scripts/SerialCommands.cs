using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public partial class SerialController
{
    /// <summary>
    /// Sends an "echo" message to device.
    /// </summary>
    [Button("Send Echo")]
    private void SendEcho()
    {
        SendSerialMessage("e");
    }

    /// <summary>
    /// Send a message to device to start sending samples.
    /// </summary>
    [Button("Initialize Sampling")]
    private void InitSampling()
    {
        SendSerialMessage("r");
    }

    /// <summary>
    /// Send a message to device to start sending samples after 1.5 seconds.
    /// </summary>
    private void InitSamplingDelayed()
    {
        StartCoroutine(InitSampleDelayedCoroutine());
    }

    private IEnumerator InitSampleDelayedCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        InitSampling();
    }

    /// <summary>
    /// Send a message to device to stop sampling.
    /// </summary>
    [Button("Stop Sampling")]
    private void StopSampling()
    {
        SendSerialMessage("f");
    }

    /// <summary>
    /// Send a message to recalibrate device.
    /// </summary>
    [Button("Recalibrate")]
    private void Recalibrate()
    {
        SendSerialMessage("c");
    }
}
