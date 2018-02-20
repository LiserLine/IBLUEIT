using System;
using System.Text;
using UnityEngine;

public abstract class Logger : MonoBehaviour
{
    [SerializeField]
    protected string FileName;

    protected bool isLogging;
    protected DateTime recordStart, recordStop;
    protected StringBuilder sb;

    public virtual void StartLogging()
    {
        if (isLogging)
            return;

        recordStart = DateTime.Now;
        isLogging = true;
    }

    public virtual void StopLogging()
    {
        if (!isLogging)
            return;

        isLogging = false;
        recordStop = DateTime.Now;
        Flush();
    }

    protected virtual void Awake() => sb = new StringBuilder();

    protected abstract void Flush();

    protected virtual void Start() => StartLogging();

    protected virtual void OnDestroy() => StopLogging();
}