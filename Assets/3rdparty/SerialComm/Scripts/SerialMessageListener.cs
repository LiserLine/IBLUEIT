using UnityEngine;

public class SerialMessageListener : MonoBehaviour
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
        // Send data
        //---------------------------------------------------------------------

        // If you press one of these keys send it to the serial device. A
        // sample serial device that accepts this input is given in the README.
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Sending E");
            _serialController.SendSerialMessage("E");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Sending R");
            _serialController.SendSerialMessage("R");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Sending F");
            _serialController.SendSerialMessage("F");
        }

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
