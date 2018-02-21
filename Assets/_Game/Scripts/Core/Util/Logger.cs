using System;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

public abstract class Logger<T> : MonoBehaviour
{
    [SerializeField]
    protected string FileName;

    protected bool isLogging;
    protected DateTime recordStart, recordStop;
    protected StringBuilder sb;

    [Button("Start Logging")]
    public virtual void StartLogging()
    {
        if (isLogging)
            return;

        sb.Clear();

        Debug.Log($"{typeof(T).Name} started. Logging...");

        recordStart = DateTime.Now;
        isLogging = true;
    }

    [Button("Stop Logging")]
    public virtual void StopLogging()
    {
        if (!isLogging)
            return;

        Debug.Log($"{typeof(T).Name} stopped. Flushing...");

        isLogging = false;
        recordStop = DateTime.Now;
        Flush();
    }

    protected virtual void Awake() => sb = new StringBuilder();

    protected abstract void Flush();

    protected virtual void Start() => StartLogging();
}