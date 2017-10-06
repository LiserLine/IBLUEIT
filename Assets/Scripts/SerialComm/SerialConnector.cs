using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SerialConnector))]
public class SerialConnector : MonoBehaviour
{
    [Tooltip("Baud rate that the serial device is using to transmit data.")]
    public int BaudRate = 9600;

    [Tooltip("After an error in the serial communication, or an unsuccessful " +
             "connect, how many milliseconds we should wait.")]
    public int ReconnectionDelay = 1000;

    [Tooltip("Maximum number of unread data messages in the queue. " +
             "New messages will be discarded.")]
    public int MaxUnreadMessages = 1;

    private SerialController _serialController;
    private bool _isConnected;
    private float _delta;

    private IEnumerator Start()
    {
        var ports = GetPortNames();

        Debug.Log($"{ports.Length} serial ports found.");

        foreach (var port in ports)
        {
            var sp = new SerialPort(port, BaudRate)
            {
                ReadTimeout = 1000,
                WriteTimeout = 1000,
                DtrEnable = true,
                RtsEnable = true,
                Handshake = Handshake.None
            };

            try
            {
                Debug.Log($"Trying to connect to {sp.PortName}:{sp.BaudRate}...");
                sp.Open();
            }
            catch (Exception e)
            {
                sp.Close();
                Debug.LogWarning($"Failed to connect to {sp.PortName}." +
                                 $"\n{e.Message}" +
                                 $"\n{e.StackTrace}");
                continue;
            }

            while (!_isConnected && _delta < 3f)
            {
                _delta += Time.deltaTime;

                sp.Write("e");

                if (sp.ReadExisting().Contains("echo"))
                {
                    _isConnected = true;
                    break;
                }

                yield return null;
            }

            _delta = 0f;

            if (!_isConnected)
                continue;

            Debug.Log($"Pitaco found on {sp.PortName}:{sp.BaudRate}");
            sp.Close();

            _serialController = GetComponent<SerialController>();
            _serialController.Connect(sp.PortName, sp.BaudRate, 1000, 1);

            break;
        }

        if (!_isConnected)
            Debug.LogWarning("Pitaco not found!");
    }

    private static string[] GetPortNames()
    {
        if (Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.OSXDashboardPlayer ||
            Application.platform == RuntimePlatform.LinuxPlayer)
        {
            return Directory.GetFiles("/dev/").Where(port => port.StartsWith("/dev/tty.usb") || port.StartsWith("/dev/ttyUSB")).ToArray();
        }

        return SerialPort.GetPortNames(); //windows
    }
}
