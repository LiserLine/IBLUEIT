using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner
    {
        #region Performance

        public int TargetsSucceeded { get; private set; }
        public int TargetsFailed { get; private set; }

        public int ObstaclesSucceeded { get; private set; }
        public int ObstaclesFailed { get; private set; }

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
                    if (airTargetsHit >= Data.Stage.Loaded.HeightLevelUpThreshold)
                    {
                        insHeightAcc += Data.Stage.Loaded.HeightIncrement;
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit++;
                    TargetsSucceeded++;
                    if (waterTargetsHit >= Data.Stage.Loaded.HeightLevelUpThreshold)
                    {
                        expHeightAcc += Data.Stage.Loaded.HeightIncrement;
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit--;
                    ObstaclesFailed++;
                    if (airObstaclesHit <= -Data.Stage.Loaded.SizeLevelDownThreshold)
                    {
                        expSizeAcc -= Data.Stage.Loaded.SizeIncrement;
                        expSizeAcc = expSizeAcc < 0 ? 0f : expSizeAcc;
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit--;
                    ObstaclesFailed++;
                    if (waterObstaclesHit <= -Data.Stage.Loaded.SizeLevelDownThreshold)
                    {
                        insSizeAcc -= Data.Stage.Loaded.SizeIncrement;
                        insSizeAcc = insSizeAcc < 0 ? 0f : insSizeAcc;
                        waterObstaclesHit = 0;
                    }
                    break;

                case "RelaxCoin":
                    relaxCoinHit++;
                    TargetsSucceeded++;
                    if (relaxCoinHit >= Data.Stage.Loaded.RelaxTimeThreshold)
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
                    if (airTargetsHit <= -Data.Stage.Loaded.HeightLevelDownThreshold)
                    {
                        insHeightAcc -= Data.Stage.Loaded.HeightIncrement;
                        insHeightAcc = insHeightAcc < 0 ? 0f : insHeightAcc;
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit--;
                    TargetsFailed++;
                    if (waterTargetsHit <= -Data.Stage.Loaded.HeightLevelDownThreshold)
                    {
                        expHeightAcc -= Data.Stage.Loaded.HeightIncrement;
                        expHeightAcc = expHeightAcc < 0 ? 0f : expHeightAcc;
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (airObstaclesHit >= Data.Stage.Loaded.SizeLevelUpThreshold)
                    {
                        expSizeAcc += Data.Stage.Loaded.SizeIncrement;
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (waterObstaclesHit >= Data.Stage.Loaded.SizeLevelUpThreshold)
                    {
                        insSizeAcc += Data.Stage.Loaded.SizeIncrement;
                        waterObstaclesHit = 0;
                    }
                    break;
            }
        }
    }
}