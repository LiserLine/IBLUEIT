﻿using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static float PitacoThreshold { get; private set; } = 7.5f;
    public static float Mercy { get; private set; } = 0.4f;

    private bool isLoaded;

    protected override void Awake()
    {
        base.Awake();

        if (isLoaded)
            return;

        LoadConstants();

        isLoaded = true;
    }

    private void LoadConstants()
    {
        var data = Utils.ReadAllText(Application.streamingAssetsPath + @"/GameSettings/Constants.csv");
        var grid = CsvParser2.Parse(data);

        PitacoThreshold = Utils.ParseFloat(grid[1][0]);
        Mercy = Utils.ParseFloat(grid[1][1]);
    }
}
