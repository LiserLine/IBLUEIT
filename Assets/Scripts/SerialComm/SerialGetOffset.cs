using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SerialController))]
public class SerialGetOffset : MonoBehaviour
{
    public static bool IsUsingOffset { get; private set; }
    public static float Offset { get; private set; }

    private SerialController _serialConnector;
    private int _count;

    public int NumberOfSamples;

    private IEnumerator Start()
    {
        _serialConnector = GetComponent<SerialController>();

        while (!_serialConnector.IsConnected)
            yield return new WaitForSeconds(3f);

        _serialConnector.OnSerialMessageReceived += OnSerialMessageReceived;

        while (!IsUsingOffset)
            yield return new WaitForSeconds(1f);

        _serialConnector.OnSerialMessageReceived -= OnSerialMessageReceived;
    }

    private void OnSerialMessageReceived(string msg)
    {
        if (IsUsingOffset) return;

        Offset += msg.Length > 1 ? float.Parse(msg.Replace('.', ',')) : 0f;
        _count++;

        if (_count != NumberOfSamples) return;

        Offset /= NumberOfSamples;
        Debug.Log($"Offset set to {Offset}");

        IsUsingOffset = true;
    }
}
