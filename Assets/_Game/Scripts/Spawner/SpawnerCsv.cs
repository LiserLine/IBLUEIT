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
            "SpawnObjects", "SpawnDelay", "GameDifficulty", "ObjectSpeed", "Target_Distance", "Target_HeightIncrement",
            "Target_LevelUp", "Target_LevelDown", "Obstacle_Distance", "Obstacle_SizeIncrement",
            "Obstacle_LevelUp", "Obstacle_LevelDown", "RelaxTimeBonusTrigger", "PlaySessionTime"
        };

        var sb = new StringBuilder();
        sb.AppendLine(items.Aggregate((a, b) => a + ";" + b));

        sb.AppendLine($"{(int)spawnObjects};{spawnDelay};{gameDifficulty};{objectSpeed};{distanceBetweenTargets};{heightIncrement};" +
                      $"{targetThresholdLevelUp};{targetThresholdLevelDown};{distanceBetweenObstacles};{sizeIncrement};" +
                      $"{obstacleThresholdLevelUp};{obstacleThresholdLevelDown};{relaxBonusTrigger};{stageManager.PlaySessionTime}");

        Utils.WriteAllText(Application.streamingAssetsPath + @"/GameSettings/stages/_new.csv", sb.ToString());
    }

    private void LoadCsv(string path)
    {
        var grid = CsvParser2.Parse(Utils.ReadAllText(path));

        spawnObjects = (EnemyType)Enum.Parse(typeof(EnemyType), grid[1][0]);
        spawnDelay = Utils.ParseFloat(grid[1][1]);
        gameDifficulty = Utils.Clip(Utils.ParseFloat(grid[1][2]), 40f, 100f);
        objectSpeed = Utils.ParseFloat(grid[1][3]);
        distanceBetweenTargets = Utils.ParseFloat(grid[1][4]);
        heightIncrement = Utils.ParseFloat(grid[1][5]);
        targetThresholdLevelUp = int.Parse(grid[1][6]);
        targetThresholdLevelDown = int.Parse(grid[1][7]);
        distanceBetweenObstacles = Utils.ParseFloat(grid[1][8]);
        sizeIncrement = Utils.ParseFloat(grid[1][9]);
        obstacleThresholdLevelUp = int.Parse(grid[1][10]);
        obstacleThresholdLevelDown = int.Parse(grid[1][11]);
        relaxBonusTrigger = int.Parse(grid[1][12]);
        stageManager.PlaySessionTime = int.Parse(grid[1][13]);
    }
}
