/**
 * SerialCommUnity (Serial Communication for Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

/* This script has many modifications from the original source. */

using System.Collections;
using UnityEngine;
using System.Threading;

/// <summary>
/// This class allows a Unity program to continually check for messages from a
/// serial device.
/// It creates a Thread that communicates with the serial port and continually
/// polls the messages on the wire.
/// That Thread puts all the messages inside a Queue, and this serialController
/// class polls that queue by means of invoking SerialThread.GetSerialMessage().
/// The serial device must send its messages separated by a newline character.
/// Neither the serialController nor the SerialThread perform any validation
/// on the integrity of the message. It's up to the one that makes sense of the
/// data.
/// </summary>
[RequireComponent(typeof(SerialConnector))]
public class SerialController : MonoBehaviour
{
    // Event to be triggered on new messages arrived from serial
    public delegate void SerialMessageHandler(string message);
    public event SerialMessageHandler OnSerialMessageReceived;

    // Variable to signalize if this component is connected to the serial device
    public bool IsConnected { get; private set; }

    [Tooltip("Initialize data request to PITACO")]
    public bool RequestPitacoData;

    // Constants used to mark the start and end of a connection. There is no
    // way you can generate clashing messages from your serial device, as I
    // compare the references of these strings, no their contents. So if you
    // send these same strings from the serial device, upon reconstruction they
    // will have different reference ids.
    public const string SerialDeviceConnected = "__Connected__";
    public const string SerialDeviceDisconnected = "__Disconnected__";

    // Internal reference to the Thread and the object that runs in it.
    protected Thread Thread;
    protected SerialThreadLines SerialThread;

    public void Connect(string portName, int BaudRate, int ReconnectionDelay, int MaxUnreadMessages)
    {
        SerialThread = new SerialThreadLines(portName,
            BaudRate,
            ReconnectionDelay,
            MaxUnreadMessages);
        Thread = new Thread(new ThreadStart(SerialThread.RunForever));
        Thread.Start();
    }

    /// <summary>
    /// Invoked whenever the serialController gameobject is deactivated.
    /// It stops and destroys the thread that was reading from the serial device.
    /// </summary>
    private void OnDisable()
    {
        // If there is a user-defined tear-down function, execute it before
        // closing the underlying COM port.
        _userDefinedTearDownFunction?.Invoke();

        // The serialThread reference should never be null at this point,
        // unless an Exception happened in the OnEnable(), in which case I've
        // no idea what face Unity will make.
        if (SerialThread != null)
        {
            SerialThread.SendMessage("f"); // Pitaco Arduino EndRequest
            SerialThread.RequestStop();
            SerialThread = null;
        }

        // This reference shouldn't be null at this point anyway.
        if (Thread == null) return;
        Thread.Join();
        Thread = null;
    }

    private void Start()
    {
        if (RequestPitacoData)
            StartCoroutine(DelayedRequestValues());
    }

    public void InitializePitacoRequest()
    {
        if (RequestPitacoData) return;
        StartCoroutine(DelayedRequestValues());
    }

    private IEnumerator DelayedRequestValues()
    {
        while (!IsConnected)
            yield return null;

        yield return new WaitForSeconds(2f);
        Debug.Log("Requesting values...");
        SendSerialMessage("r");
        RequestPitacoData = true;
    }

    /// <summary>
    /// Polls messages from the queue that the SerialThread object keeps. Once a
    /// message has been polled it is removed from the queue. There are some
    /// special messages that mark the start/end of the communication with the
    /// device.
    /// </summary>
    private void Update()
    {
        // Read the next message from the queue
        var message = (string)SerialThread?.ReadMessage();
        if (message == null)
            return;

        // Check if the message is plain data or a connect/disconnect event.
        if (ReferenceEquals(message, SerialDeviceConnected))
            OnConnectionEvent(true);
        else if (ReferenceEquals(message, SerialDeviceDisconnected))
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
        //_requestingValues = false;
        Debug.Log("Connection attempt failed or disconnection detected!");
    }

    /// <summary>
    /// Returns a new unread message from the serial device. You only need to
    /// call this if you don't provide a message listener.
    /// </summary>
    /// <returns></returns>
    public string ReadSerialMessage()
    {
        // Read the next message from the queue
        return (string)SerialThread?.ReadMessage();
    }

    /// <summary>
    /// Puts a message in the outgoing queue. The thread object will send the
    /// message to the serial device when it considers it's appropriate.
    /// </summary>
    /// <param name="message">Message to be sent via serial</param>
    public void SendSerialMessage(string message)
    {
        SerialThread.SendMessage(message);
    }

    /// <summary>
    /// Executes a user-defined function before Unity closes the COM port, so
    /// the user can send some tear-down message to the hardware reliably.
    /// </summary>
    public delegate void TearDownFunction();
    private TearDownFunction _userDefinedTearDownFunction;
    public void SetTearDownFunction(TearDownFunction userFunction)
    {
        this._userDefinedTearDownFunction = userFunction;
    }
}
