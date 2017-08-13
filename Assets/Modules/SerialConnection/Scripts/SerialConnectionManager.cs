/*
 * This class is used with the arduino code inside "StreamingAssets/arduino-source"
 */

using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialConnectionManager : MonoBehaviour
{
    public static SerialConnectionManager Instance;

    private float _sensorValue = 0f;
    private float _offsetValue = 0f;

    private Thread _requestValuesThread;
    private bool _requestValuesLooping;

    private Thread _keepAliveThread;
    private bool _keepAliveLooping;

    private SerialPort _serialPort;
    private bool _serialConnected = false;

    private bool _isRequestEnabled = false;
    private bool _isReady = false;

    #region Properties

    public float SensorValue { get { return _sensorValue - _offsetValue; } }
    public bool IsReady { get { return _isReady; } }
    public bool IsConnected { get { return _serialConnected; } }
    public bool IsOpen { get { return _serialPort.IsOpen; } }
    public bool IsRequestEnabled { get { return _isRequestEnabled; } }

    #endregion

    [Header("Settings")]
    public bool ShowSensorValues = false;
    public int BaudRate = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("SerialConnectionManager awaking...");
        DontDestroyOnLoad(gameObject);

        StartThreads();

        _isReady = true;
        Debug.Log("SerialConnectionManager ready.");
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    #region Device Connection

    private float GetSensorValue()
    {
        _serialPort.Write("r");
        string receivedLine = _serialPort.ReadLine();
        receivedLine = receivedLine.Replace('.', ',');
        return float.Parse(receivedLine);
    }

    public void Connect()
    {
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            _serialPort = new SerialPort(port, BaudRate);
            ConnectDevice();
            if (_serialConnected) break;
        }
    }

    private void ConnectDevice()
    {
        Debug.Log("Connecting device on " + _serialPort.PortName + ":" + _serialPort.BaudRate);

        try
        {
            Open();
            CheckDeviceIdentity();
            _serialConnected = true;
            Debug.Log("Device connected.");
            GetOffset();
        }
        catch (Exception ex)
        {
            Dispose();
            Debug.LogWarning(ex.Message);
        }
    }

    private void Open()
    {
        _serialPort.ReadTimeout = 30;
        _serialPort.Open();
    }

    private void CheckDeviceIdentity()
    {
        Debug.LogFormat("Checking {0}:{1} identity...", _serialPort.PortName, _serialPort.BaudRate);

        Thread.Sleep(1500); //This is needed to avoid timeout

        _serialPort.Write("e");
        string msg = _serialPort.ReadLine();

        if (!msg.Equals("echo"))
            throw new Exception("Unknown device!");
    }

    private void GetOffset()
    {
        //ToDo - make it a better code

        int count = 0;
        float mean = 40f;
        float tmpOffset = 0f;

        while (count < mean)
        {
            if (!_serialConnected)
                continue;

            try { tmpOffset += GetSensorValue(); }
            //catch (TimeoutException) { }
            //catch (IOException) { }
            //catch (Exception ex) { Debug.LogErrorFormat("{0}: {1}", ex.GetType(), ex.Message); }
            catch { }

            count++;
        }

        _offsetValue = tmpOffset / (mean / 2); // mean because something goes wrong in this method

        Debug.LogFormat("Offset: {0}", _offsetValue);
    }

    public void Disconnect(bool reconnect = false)
    {
        Dispose();

        if (!reconnect)
            StopThreads();

        Debug.Log("Device disconnected.");
    }

    public void Reconnect()
    {
        Debug.LogWarning("Reconnecting device...");
        _serialConnected = false;
    }

    private void Dispose()
    {
        _serialConnected = false;

        if (_serialPort != null)
        {
            _serialPort.Close();
            _serialPort.Dispose();
            _serialPort = null;
        }
    }

    public void EnableRequest()
    {
        _isRequestEnabled = true;
    }

    public void DisabeRequest()
    {
        _isRequestEnabled = false;
    }

    #endregion

    #region Threads

    private void StartThreads()
    {
        Debug.Log("Starting threads...");

        if (_requestValuesThread == null)
        {
            _requestValuesThread = new Thread(new ThreadStart(RequestValuesLoop));
            _requestValuesLooping = true;
            _requestValuesThread.Start();
        }

        if (_keepAliveThread == null)
        {
            _keepAliveThread = new Thread(new ThreadStart(KeepAliveLoop));
            _keepAliveLooping = true;
            _keepAliveThread.Start();
        }
    }

    private void StopThreads()
    {
        Debug.Log("Stopping threads...");

        if (_requestValuesThread != null)
        {
            _requestValuesThread.Abort();
            _requestValuesLooping = false;
            _requestValuesThread = null;
        }

        if (_keepAliveThread != null)
        {
            _keepAliveThread.Abort();
            _keepAliveLooping = false;
            _keepAliveThread = null;
        }
    }

    private void RequestValuesLoop()
    {
        while (_requestValuesLooping)
        {
            if (!_isRequestEnabled || !_serialConnected || _offsetValue == 0)
                continue;

            try
            {
                _sensorValue = GetSensorValue();
                if (ShowSensorValues) Debug.LogFormat("SensorValue: {0}", _sensorValue);
            }
            catch (TimeoutException) { }
            catch (IOException) { _serialConnected = false; }
            catch (Exception ex)
            {
                _serialConnected = false;
                Debug.LogErrorFormat("{0}: {1}", ex.GetType(), ex.Message);
            }

            Thread.Sleep(1);
        }
    }


    private void KeepAliveLoop()
    {
        while (_keepAliveLooping)
        {
            if (!_serialConnected)
                Connect();

            Thread.Sleep(1500);
        }
    }

    #endregion

}
