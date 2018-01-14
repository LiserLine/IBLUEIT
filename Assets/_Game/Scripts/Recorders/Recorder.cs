using System;
using UnityEngine;

public abstract class Recorder<T> : MonoBehaviour where T : MonoBehaviour
{
    protected DateTime recordStart, recordStop;
    protected bool isRecording;

    [SerializeField]
    protected string FileName;

    protected virtual void StartRecord()
    {
        recordStart = DateTime.Now;
        isRecording = true;
    }

    protected virtual void StopRecord()
    {
        recordStop = DateTime.Now;
        isRecording = false;
    }
}
