using UnityEngine;

public partial class GameManager : Singleton<GameManager>
{
    public static float PitacoThreshold { get; private set; } = 7.5f;
    public static float Mercy { get; private set; } = 0.4f;
    public static float PlataformMinScoreMultiplier { get; private set; } = 0.7f;

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
        PlataformMinScoreMultiplier = Mathf.Clamp(Utils.ParseFloat(grid[1][2]), 0.5f, 1f);
    }
}
