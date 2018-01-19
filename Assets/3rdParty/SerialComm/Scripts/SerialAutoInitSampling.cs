using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Attach this to a gameobject to autostart sampling the device.
/// </summary>
public class SerialAutoInitSampling : MonoBehaviour
{
    [SerializeField]
    private SerialController serialController;

    private void Awake()
    {
        if (serialController == null)
            serialController = FindObjectOfType<SerialController>();

        if(serialController == null)
            throw new Exception("Serial Controller not found in scene!");

        serialController.OnSerialConnected += AutoSample;
    }

    private void AutoSample() => StartCoroutine(InitSampleDelayedCoroutine());

    private IEnumerator InitSampleDelayedCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        serialController.InitSampling();
    }
}
