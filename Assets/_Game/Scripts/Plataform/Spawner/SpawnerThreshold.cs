using UnityEngine;

public partial class Spawner
{
    #region Performance

    public int TargetsSucceeded { get; private set; }
    public int TargetsFailed { get; private set; }

    public int ObstaclesSucceeded { get; private set; }
    public int ObstaclesFailed { get; private set; }

    //public int AirTargetsHitSuccess { get; private set; }
    //public int AirTargetsHitFail { get; private set; }
    //public int WaterTargetsHitSuccess { get; private set; }
    //public int WaterTargetsHitFail { get; private set; }

    //public int AirObstaclesHitSuccess { get; private set; }
    //public int AirObstaclesHitFail { get; private set; }
    //public int WaterObstaclesHitSuccess { get; private set; }
    //public int WaterObstaclesHitFail { get; private set; }

    #endregion Performance

    private int airTargetsHit;
    private int airObstaclesHit;
    private int waterTargetsHit;
    private int waterObstaclesHit;
    private int relaxCoinHit;

    private void Player_OnEnemyHit(GameObject hit)
    {
        switch (hit.tag)
        {
            case "AirTarget":
                airTargetsHit++;
                TargetsSucceeded++;
                if (airTargetsHit >= Stage.Loaded.HeightLevelUpThreshold)
                {
                    insHeightAcc += Stage.Loaded.HeightIncrement;
                    airTargetsHit = 0;
                }
                break;

            case "WaterTarget":
                waterTargetsHit++;
                TargetsSucceeded++;
                if (waterTargetsHit >= Stage.Loaded.HeightLevelUpThreshold)
                {
                    expHeightAcc += Stage.Loaded.HeightIncrement;
                    waterTargetsHit = 0;
                }
                break;

            case "AirObstacle":
                airObstaclesHit--;
                ObstaclesFailed++;
                if (airObstaclesHit <= -Stage.Loaded.SizeLevelDownThreshold)
                {
                    insSizeAcc -= Stage.Loaded.SizeIncrement;
                    airObstaclesHit = 0;
                }
                break;

            case "WaterObstacle":
                waterObstaclesHit--;
                ObstaclesFailed++;
                if (waterObstaclesHit <= -Stage.Loaded.SizeLevelDownThreshold)
                {
                    expSizeAcc -= Stage.Loaded.SizeIncrement;
                    waterObstaclesHit = 0;
                }
                break;

            case "RelaxCoin":
                relaxCoinHit++;
                TargetsSucceeded++;
                if (relaxCoinHit >= Stage.Loaded.RelaxTimeThreshold)
                    spawnRelaxTime = true;
                break;
        }
    }

    public void Player_OnEnemyMiss(string objectTag)
    {
        switch (objectTag)
        {
            case "AirTarget":
                airTargetsHit--;
                TargetsFailed++;
                if (airTargetsHit <= -Stage.Loaded.HeightLevelDownThreshold)
                {
                    insHeightAcc -= Stage.Loaded.HeightIncrement;
                    airTargetsHit = 0;
                }
                break;

            case "WaterTarget":
                waterTargetsHit--;
                TargetsFailed++;
                if (waterTargetsHit <= -Stage.Loaded.HeightLevelDownThreshold)
                {
                    expHeightAcc -= Stage.Loaded.HeightIncrement;
                    waterTargetsHit = 0;
                }
                break;

            case "AirObstacle":
                airObstaclesHit++;
                ObstaclesSucceeded++;
                if (airObstaclesHit >= Stage.Loaded.SizeLevelUpThreshold)
                {
                    insSizeAcc += Stage.Loaded.SizeIncrement;
                    airObstaclesHit = 0;
                }
                break;

            case "WaterObstacle":
                waterObstaclesHit++;
                ObstaclesSucceeded++;
                if (waterObstaclesHit >= Stage.Loaded.SizeLevelUpThreshold)
                {
                    expSizeAcc += Stage.Loaded.SizeIncrement;
                    waterObstaclesHit = 0;
                }
                break;
        }
    }
}