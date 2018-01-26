﻿using UnityEngine;
using System;

[Serializable]
public class Stat
{
    [SerializeField]
    private BarScript bar;

    [SerializeField]
    private float maxVal;

    [SerializeField]
    private float currentVal;

    public float CurrentVal
    {
        get { return currentVal; }
        set
        {
            this.currentVal = Mathf.Clamp(value, 0, MaxVal);
            bar.value = currentVal;
        }
    }

    public float MaxVal
    {
        get { return maxVal; }
        set
        {
            bar.maxValue = value;
            this.maxVal = value;
        }
    }

    public void Initialize()
    {
        this.MaxVal = maxVal;
        this.CurrentVal = currentVal;
    }
}