/**
 * SerialCommUnity (Serial Communication for Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Ibit.Core.Util;
using UnityEngine;

/**
 * This class allows a Unity program to continually check for messages from a
 * serial device.
 *
 * It creates a Thread that communicates with the serial port and continually
 * polls the messages on the wire.
 * That Thread puts all the messages inside a Queue, and this SerialController
 * class polls that queue by means of invoking SerialThread.GetSerialMessage().
 *
 * The serial device must send its messages separated by a newline character.
 * Neither the SerialController nor the SerialThread perform any validation
 * on the integrity of the message. It's up to the one that makes sense of the
 * data.
 */

namespace Ibit.Core.Serial
{
    public partial class SerialController : MonoBehaviour
    {
        // Constants used to mark the start and end of a connection. There is no
        // way you can generate clashing messages from your serial device, as I
        // compare the references of these strings, no their contents. So if you
        // send these same strings from the serial device, upon reconstruction they
        // will have different reference ids.
        public const string SERIAL_DEVICE_CONNECTED = "__Connected__";

        public const string SERIAL_DEVICE_DISCONNECTED = "__Disconnected__";

        [SerializeField]
        [Tooltip("Baud rate that the serial device is using to transmit data.")]
        private int baudRate = 115200;

        [SerializeField]
        [Tooltip("Maximum number of unread data messages in the queue. " +
                 "New messages will be discarded.")]
        private int maxUnreadMessages = 1;

        [SerializeField]
        [Tooltip("After an error in the serial communication, or an unsuccessful " +
                 "connect, how many milliseconds we should wait.")]
        private int reconnectionDelay = 1000;

        private SerialThread serialThread;

        // Internal reference to the Thread and the object that runs in it.
        private Thread thread;

        public delegate void SerialConnectedHandler();

        public delegate void SerialDisconnectedHandler();

        public delegate void SerialMessageReceivedHandler(string msg);

        // ------------------------------------------------------------------------
        // Executes a user-defined function before Unity closes the COM port, so
        // the user can send some tear-down message to the hardware reliably.
        // ------------------------------------------------------------------------
        public delegate void TearDownFunction();

        private TearDownFunction userDefinedTearDownFunction;

        public event SerialConnectedHandler OnSerialConnected;

        public event SerialDisconnectedHandler OnSerialDisconnected;

        public event SerialMessageReceivedHandler OnSerialMessageReceived;

        public bool IsConnected { get; private set; }

        // ------------------------------------------------------------------------
        // Returns a new unread message from the serial device. You only need to
        // call this if you preferrred to not provide a message listener.
        // ------------------------------------------------------------------------
        public string ReadSerialMessage() => serialThread.ReadSerialMessage();

        // ------------------------------------------------------------------------
        // Puts a message in the outgoing queue. The thread object will send the
        // message to the serial device when it considers it appropriate.
        // ------------------------------------------------------------------------
        public void SendSerialMessage(string message)
        {
            if (!IsConnected)
                return;

            serialThread.SendSerialMessage(message);
        }

        public void SetTearDownFunction(TearDownFunction userFunction) => this.userDefinedTearDownFunction = userFunction;

        /// <summary>
        /// Get Ports avaiable.
        /// </summary>
        /// <returns></returns>
        private static string[] GetPortNames()
        {
            if (Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.LinuxPlayer ||
                Application.platform == RuntimePlatform.Android)
            {
                return Directory.GetFiles("/dev/").Where(port => port.StartsWith("/dev/tty.usb") || port.StartsWith("/dev/ttyUSB")).ToArray();
            }

            return SerialPort.GetPortNames(); //windows
        }

        /// <summary>
        /// Autoconnects the first device that answers "echo". Adaptation for the game.
        /// </summary>
        /// <returns></returns>
        private string AutoConnect()
        {
            var ports = GetPortNames();

            if (ports.Length < 1)
                SysMessage.Warning("PITACO não encontrado!");

            foreach (var port in ports)
            {
                var sp = new SerialPort(port, baudRate)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                    DtrEnable = true,
                    RtsEnable = true,
                    Handshake = Handshake.None
                };

                try
                {
                    sp.Open();
                    Thread.Sleep(1500);
                    sp.Write("e");

                    if (!sp.ReadLine().Contains("echo"))
                        throw new TimeoutException("No response from device.");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Unable to connect {sp.PortName}:{sp.BaudRate}.\n{e.GetType()}: {e.Message}");
                    sp.Close();
                    sp.Dispose();
                    continue;
                }

                sp.Close();
                sp.Dispose();

                return sp.PortName;
            }

            return null;
        }

        private void Awake() => Connect();

        private void Connect()
        {
            var portName = AutoConnect();

            if (string.IsNullOrEmpty(portName))
            {
                Debug.LogWarning("Failed to connect to a serial device!");
                return;
            }

            serialThread = new SerialThread(portName, baudRate, reconnectionDelay, maxUnreadMessages);

            thread = new Thread(serialThread.RunForever);
            thread.Start();

            IsConnected = true;

            Debug.Log($"Connected to {portName}:{baudRate}");
        }

        private void Disconnect()
        {
            if (!IsConnected)
                return;

            // If there is a user-defined tear-down function, execute it before
            // closing the underlying COM port.
            userDefinedTearDownFunction?.Invoke();

            StopSampling();

            // The serialThread reference should never be null at this point,
            // unless an Exception happened in the OnEnable(), in which case I've
            // no idea what face Unity will make.
            if (serialThread != null)
            {
                serialThread.RequestStop();
                serialThread = null;
            }

            // This reference shouldn't be null at this point anyway.
            if (thread != null)
            {
                thread.Join();
                thread = null;
            }

            IsConnected = false;

            Debug.Log("Serial disconnected.");
        }

        private void OnDestroy() => Disconnect();

        private void Update()
        {
            // Read the next message from the queue
            var message = serialThread?.ReadSerialMessage();
            if (message == null)
                return;

            // Check if the message is plain data or a connect/disconnect event.
            if (string.Equals(message, SERIAL_DEVICE_CONNECTED))
            {
                IsConnected = true;
                OnSerialConnected?.Invoke();
            }
            else if (string.Equals(message, SERIAL_DEVICE_DISCONNECTED))
            {
                IsConnected = false;
                OnSerialDisconnected?.Invoke();
            }
            else
            {
                if (!IsConnected)
                    return;

                OnSerialMessageReceived?.Invoke(message);
            }
        }
    }
}