using UnityEngine;

public class SerialMessageWriter : MonoBehaviour
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Sending R");
            _serialController.SendSerialMessage("R");
        }
    }
}
