using System.Collections;
using UnityEngine;

public class SerialListener : MonoBehaviour
{
    private SerialController _serialController;
    private bool _requestingValues;
    private Coroutine _requestCoroutine;

    public bool IsConnected { get; private set; }

    public string MessageReceived;
    public bool RequestValues = false;

    private IEnumerator DelayedRequestValues()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Requesting values...");
        _serialController.SendSerialMessage("r");
    }

    private void Start()
    {
        _serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    private void Update()
    {
        if (RequestValues && IsConnected && !_requestingValues)
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
        MessageReceived = message;
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
}
