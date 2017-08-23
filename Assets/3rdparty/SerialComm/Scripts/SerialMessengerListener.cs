using System.Collections;
using UnityEngine;

public class SerialMessengerListener : MonoBehaviour
{
    private SerialController _serialController;

    public string MessageReceived { get; private set; }
    public bool IsConnected { get; private set; }

    public bool RequestValuesOnStart = false;

    private IEnumerator DelayedInitSerial()
    {
        yield return new WaitForSeconds(4f);
        _serialController.SendSerialMessage("r");
    }
    
    private void Start()
    {
        _serialController = GameObject.Find("SerialController").GetComponent<SerialController>();

        if (RequestValuesOnStart)
            StartCoroutine(DelayedInitSerial());
    }
    
    private void Update()
    {
        var message = _serialController.ReadSerialMessage();
        if (message == null)
            return;

        if (ReferenceEquals(message, SerialController.SerialDeviceConnected))
        {
            IsConnected = true;
            Debug.Log("Connection established");
        }
        else if (ReferenceEquals(message, SerialController.SerialDeviceDisconnected))
        {
            IsConnected = false;
            Debug.Log("Connection attempt failed or disconnection detected");
        }
        else
            MessageReceived = message;
    }
}
