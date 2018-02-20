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

    [Button]
    public virtual void StartLogging()
    {
        if (isLogging)
            return;

        sb.Clear();

        Debug.Log($"{typeof(T)} started. Logging...");

        recordStart = DateTime.Now;
        isLogging = true;
    }

    [Button]
    public virtual void StopLogging()
    {
        if (!isLogging)
            return;

        Debug.Log($"{typeof(T)} stopped. Flusing...");

        isLogging = false;
        recordStop = DateTime.Now;
        Flush();
    }

    protected virtual void Awake() => sb = new StringBuilder();

    protected abstract void Flush();

    protected virtual void Start() => StartLogging();
}