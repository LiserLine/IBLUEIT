using System;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class Spawner
{
    [ContextMenu("Write Settings")]
    private void WriteCsv()
    {
        var items = new[]
        {
            "StageId", "SpawnObjects", "SpawnDelay", "GameDifficulty", "ObjectSpeed", "Target_Distance", "Target_HeightIncrement",
            "Target_LevelUp", "Target_LevelDown", "Obstacle_Distance", "Obstacle_SizeIncrement",
            "Obstacle_LevelUp", "Obstacle_LevelDown", "RelaxTimeBonusTrigger", "PlaySessionTime"
        };

        var sb = new StringBuilder();
        sb.AppendLine(items.Aggregate((a, b) => a + ";" + b));

        sb.AppendLine($"{0};{(int)spawnObjects};{spawnDelay};{gameDifficulty};{objectSpeed};{distanceBetweenTargets};{heightIncrement};" +
                      $"{targetThresholdLevelUp};{targetThresholdLevelDown};{distanceBetweenObstacles};{sizeIncrement};" +
                      $"{obstacleThresholdLevelUp};{obstacleThresholdLevelDown};{relaxBonusTrigger};{stageManager.PlaySessionTime}");

        Utils.WriteAllText(Application.streamingAssetsPath + @"/GameSettings/NewStageInfo.csv", sb.ToString());
    }

    private void LoadCsv(int id)
    {
        var stageListPath = Utils.ReadAllText(Application.streamingAssetsPath + @"/GameSettings/StageList.csv");
        var grid = CsvParser2.Parse(stageListPath);

        spawnObjects = (EnemyType)Enum.Parse(typeof(EnemyType), grid[id][1]);
        spawnDelay = Utils.ParseFloat(grid[id][2]);
        gameDifficulty = Utils.Clip(Utils.ParseFloat(grid[id][3]), gameDifficulties[0], 100f);
        objectSpeed = Utils.ParseFloat(grid[id][4]);
        distanceBetweenTargets = Utils.ParseFloat(grid[id][5]);
        heightIncrement = Utils.ParseFloat(grid[id][6]);
        targetThresholdLevelUp = int.Parse(grid[id][7]);
        targetThresholdLevelDown = int.Parse(grid[id][8]);
        distanceBetweenObstacles = Utils.ParseFloat(grid[id][9]);
        sizeIncrement = Utils.ParseFloat(grid[id][10]);
        obstacleThresholdLevelUp = int.Parse(grid[id][11]);
        obstacleThresholdLevelDown = int.Parse(grid[id][12]);
        relaxBonusTrigger = int.Parse(grid[id][13]);
        stageManager.PlaySessionTime = int.Parse(grid[id][14]);

        Debug.Log($"Information loaded for stage {id}.");
    }
}
