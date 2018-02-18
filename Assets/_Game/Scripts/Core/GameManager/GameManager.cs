﻿using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private static bool isLoaded;
    public static float CapacityMultiplier { get; private set; } = 0.5f;
    public static float LevelUnlockScoreThreshold { get; private set; } = 0.7f;
    public static float PitacoFlowThreshold { get; private set; } = 7.5f;

#if UNITY_EDITOR
    public void QuitGame() => Debug.Log("Quit Game pressed!");
#else
    public void QuitGame() => System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (isLoaded)
            return;

        LoadGlobals();

        isLoaded = true;
    }

    private void LoadGlobals()
    {
        var data = FileReader.ReadCsv(Application.streamingAssetsPath + @"/GameSettings/Constants.csv");
        var grid = CsvParser2.Parse(data);

        PitacoFlowThreshold = Parsers.Float(grid[1][0]);
        CapacityMultiplier = Parsers.Float(grid[1][1]);
        LevelUnlockScoreThreshold = Mathf.Clamp(Parsers.Float(grid[1][2]), 0.5f, 1f);
    }
}
