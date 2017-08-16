using UnityEngine;

public class SerialMessageReader : MonoBehaviour
{
    private SerialController _serialController;

    // Use this for initialization
    private void Start()
    {
        _serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    // Update is called once per frame
    private void Update()
    {
        //---------------------------------------------------------------------
        // Receive data
        //---------------------------------------------------------------------

        string message = _serialController.ReadSerialMessage();

        if (message == null)
            return;

        // Check if the message is plain data or a connect/disconnect event.
        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
            Debug.Log("Connection established");
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");
        else
            Debug.Log("Message arrived: " + message);
    }
}
