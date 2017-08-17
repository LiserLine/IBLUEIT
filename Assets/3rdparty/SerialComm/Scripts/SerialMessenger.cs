using System.Collections;
using UnityEngine;

public class SerialMessenger : MonoBehaviour
{
    private SerialController _serialController;

    public string MessageReceived;

    private IEnumerator DelayedInitSerial()
    {
        yield return new WaitForSeconds(4f);
        _serialController.SendSerialMessage("r");
    }
    
    private void Start()
    {
        _serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        StartCoroutine(DelayedInitSerial());
    }
    
    private void Update()
    {
        string message = _serialController.ReadSerialMessage();
        if (message == null)
            return;

        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
            Debug.Log("Connection established");
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");
        else
            MessageReceived = message;
    }
}
