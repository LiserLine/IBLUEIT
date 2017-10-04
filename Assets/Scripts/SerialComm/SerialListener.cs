using System.Collections;
using UnityEngine;

public class SerialListener : MonoBehaviour
{
    private SerialController _serialController;
    private bool _requestingValues;
    private Coroutine _requestCoroutine;

    public bool IsConnected { get; private set; }

    public delegate void SerialMessageHandler(string messageArrived);
    public event SerialMessageHandler OnSerialMessageReceived;

    [Header("Controls")]
    public bool EnableRequestValues;

    private IEnumerator DelayedRequestValues()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Requesting values...");
        _serialController.SendSerialMessage("r");
    }

    private void Awake()
    {
        _serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    private void Update()
    {
        if (EnableRequestValues && IsConnected && !_requestingValues)
        {
            _requestingValues = true;
            if (_requestCoroutine != null)
            {
                StopCoroutine(_requestCoroutine);
            }
            _requestCoroutine = StartCoroutine(DelayedRequestValues());
        }

        var message = _serialController?.ReadSerialMessage();
        if (message == null)
            return;

        if (ReferenceEquals(message, SerialController.SerialDeviceConnected))
            OnConnectionEvent(true);
        else if (ReferenceEquals(message, SerialController.SerialDeviceDisconnected))
            OnConnectionEvent(false);
        else
            OnMessageArrived(message);
    }

    public void OnConnectionEvent(bool status)
    {
        if (status)
            OnConnection();
        else
            OnDisconnection();
    }

    public void OnMessageArrived(string message)
    {
        OnSerialMessageReceived?.Invoke(message);
    }

    private void OnConnection()
    {
        IsConnected = true;
        Debug.Log("Connection established!");
    }

    private void OnDisconnection()
    {
        IsConnected = false;
        _requestingValues = false;
        Debug.Log("Connection attempt failed or disconnection detected!");
    }

    /// <summary>
    /// This will check editor's EnableRequestValues and will start the DelayedRequestValues
    /// </summary>
    public void InitValueRequest()
    {
        if (_requestingValues)
        {
            Debug.LogWarning("Already requesting values!");
            return;
        }

        EnableRequestValues = true;
    }
}
