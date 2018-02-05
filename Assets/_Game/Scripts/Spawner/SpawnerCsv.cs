using System;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class Spawner
{
    [ContextMenu("Save Settings")]
    private void WriteCsv()
    {
        var items = new[]
        {
            "StageId", "SpawnObjects", "SpawnDelay", "GameDifficulty", "ObjectSpeed", "Target_HeightIncrement",
            "Target_LevelUp", "Target_LevelDown", "Obstacle_SizeIncrement",
            "Obstacle_LevelUp", "Obstacle_LevelDown", "RelaxTimeBonusTrigger", "PlaySessionTime"
        };

        var sb = new StringBuilder();
        sb.AppendLine(items.Aggregate((a, b) => a + ";" + b));

        sb.AppendLine($"{0};{(int)spawnObjects};{spawnDelay};{gameDifficulty};{objectSpeed};{heightIncrement};" +
                              $"{targetThresholdLevelUp};{targetThresholdLevelDown};{sizeIncrement};" +
                              $"{obstacleThresholdLevelUp};{obstacleThresholdLevelDown};{relaxBonusTrigger};{StageManager.Instance.PlaySessionTime}");

        Utils.WriteAllText(Application.streamingAssetsPath + @"/GameSettings/NewStageInfo.csv", sb.ToString());
    }

    private void LoadCsv(int id)
    {
        var stageListPath = Utils.ReadAllText(Application.streamingAssetsPath + @"/GameSettings/StageList.csv");

        if (stageListPath.Split('\t').Length > 0)
            stageListPath = stageListPath.Replace('\t', ';');

        var grid = CsvParser2.Parse(stageListPath);

        spawnObjects = (EnemyType)Enum.Parse(typeof(EnemyType), grid[id][1]);
        spawnDelay = Utils.ParseFloat(grid[id][2]);
        gameDifficulty = Mathf.Clamp(Utils.ParseFloat(grid[id][3]), gameDifficulties[0], 100f);
        objectSpeed = Utils.ParseFloat(grid[id][4]);
        heightIncrement = Utils.ParseFloat(grid[id][5]);
        targetThresholdLevelUp = int.Parse(grid[id][6]);
        targetThresholdLevelDown = int.Parse(grid[id][7]);
        sizeIncrement = Utils.ParseFloat(grid[id][8]);
        obstacleThresholdLevelUp = int.Parse(grid[id][9]);
        obstacleThresholdLevelDown = int.Parse(grid[id][10]);
        relaxBonusTrigger = int.Parse(grid[id][11]);
        StageManager.Instance.PlaySessionTime = int.Parse(grid[id][12]);

        Debug.Log($"Stage {id} loaded.");
    }
}
