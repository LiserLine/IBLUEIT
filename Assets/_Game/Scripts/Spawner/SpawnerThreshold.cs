using UnityEngine;

public partial class Spawner
{
    private int airTargetsHit;
    private int airObstaclesHit;
    private int waterTargetsHit;
    private int waterObstaclesHit;

    private void Player_OnEnemyHit(GameObject hit)
    {
        switch (hit.tag)
        {
            case "AirTarget":
                airTargetsHit++;
                if (airTargetsHit >= targetThresholdLevelUp)
                {
                    insHeightAccumulator += heightIncrement;
                    airTargetsHit = 0;
                }
                break;

            case "WaterTarget":
                waterTargetsHit++;
                if (waterTargetsHit >= targetThresholdLevelUp)
                {
                    expHeightAccumulator += heightIncrement;
                    waterTargetsHit = 0;
                }
                break;

            case "AirObstacle":
                airObstaclesHit--;
                if (airObstaclesHit <= -obstacleThresholdLevelDown)
                {
                    insSizeAccumulator -= sizeIncrement;
                    airObstaclesHit = 0;
                }
                break;

            case "WaterObstacle":
                waterObstaclesHit--;
                if (waterObstaclesHit <= -obstacleThresholdLevelDown)
                {
                    expSizeAccumulator -= sizeIncrement;
                    waterObstaclesHit = 0;
                }
                break;
        }
    }

    private void Player_OnEnemyMiss(GameObject miss)
    {
        switch (miss.tag)
        {
            case "AirTarget":
                airTargetsHit--;
                if (airTargetsHit <= -targetThresholdLevelUp)
                {
                    insHeightAccumulator -= heightIncrement;
                    airTargetsHit = 0;
                }
                break;

            case "WaterTarget":
                waterTargetsHit--;
                if (waterTargetsHit <= -targetThresholdLevelDown)
                {
                    expHeightAccumulator -= heightIncrement;
                    waterTargetsHit = 0;
                }
                break;

            case "AirObstacle":
                airObstaclesHit++;
                if (airObstaclesHit >= obstacleThresholdLevelUp)
                {
                    insSizeAccumulator += sizeIncrement;
                    airObstaclesHit = 0;
                }
                break;

            case "WaterObstacle":
                waterObstaclesHit++;
                if (waterObstaclesHit >= obstacleThresholdLevelUp)
                {
                    expSizeAccumulator += sizeIncrement;
                    waterObstaclesHit = 0;
                }
                break;
        }
    }
}
