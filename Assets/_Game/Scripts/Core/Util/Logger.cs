using System;
using System.Text;
using UnityEngine;

public abstract class Logger : MonoBehaviour
{
    [SerializeField]
    protected string FileName;

    protected bool isRecording;
    protected DateTime recordStart, recordStop;
    protected StringBuilder sb;

    public virtual void StartLogging()
    {
        recordStart = DateTime.Now;
        isRecording = true;
    }

    public virtual void StopLogging()
    {
        recordStop = DateTime.Now;
        isRecording = false;
        Flush();
    }

    protected virtual void Awake() => sb = new StringBuilder();

    protected abstract void Flush();

    protected virtual void Start() => StartLogging();
}