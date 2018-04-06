using NaughtyAttributes;
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

        [ShowNonSerializedField] private float expHeightAcc;
        [ShowNonSerializedField] private float expSizeAcc;
        [ShowNonSerializedField] private float insHeightAcc;
        [ShowNonSerializedField] private float insSizeAcc;

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
                        IncrementInsHeightAcc();
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit++;
                    TargetsSucceeded++;
                    if (waterTargetsHit >= Data.Stage.Loaded.HeightLevelUpThreshold)
                    {
                        IncrementExpHeightAcc();
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit--;
                    ObstaclesFailed++;
                    if (airObstaclesHit <= -Data.Stage.Loaded.SizeLevelDownThreshold)
                    {
                        DecrementExpSizeAcc();
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit--;
                    ObstaclesFailed++;
                    if (waterObstaclesHit <= -Data.Stage.Loaded.SizeLevelDownThreshold)
                    {
                        DecrementInsSizeAcc();
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
                        DecrementInsHeightAcc();
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit--;
                    TargetsFailed++;
                    if (waterTargetsHit <= -Data.Stage.Loaded.HeightLevelDownThreshold)
                    {
                        DecrementExpHeightAcc();
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (airObstaclesHit >= Data.Stage.Loaded.SizeLevelUpThreshold)
                    {
                        IncrementExpSizeAcc();
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (waterObstaclesHit >= Data.Stage.Loaded.SizeLevelUpThreshold)
                    {
                        IncrementInsSizeAcc();
                        waterObstaclesHit = 0;
                    }
                    break;
            }
        }

        public void IncrementInsSizeAcc()
        {
            insSizeAcc += Data.Stage.Loaded.SizeIncrement;
        }

        public void DecrementInsSizeAcc()
        {
            insSizeAcc -= Data.Stage.Loaded.SizeIncrement;
            insSizeAcc = insSizeAcc < 0f ? 0f : insSizeAcc;
        }

        public void IncrementInsHeightAcc()
        {
            insHeightAcc += Data.Stage.Loaded.HeightIncrement;
        }

        public void DecrementInsHeightAcc()
        {
            insHeightAcc -= Data.Stage.Loaded.HeightIncrement;
            insHeightAcc = insHeightAcc < 0f ? 0f : insHeightAcc;
        }

        public void IncrementExpSizeAcc()
        {
            expSizeAcc += Data.Stage.Loaded.SizeIncrement;
        }

        public void DecrementExpSizeAcc()
        {
            expSizeAcc -= Data.Stage.Loaded.SizeIncrement;
            expSizeAcc = expSizeAcc < 0f ? 0f : expSizeAcc;
        }

        public void IncrementExpHeightAcc()
        {
            expHeightAcc += Data.Stage.Loaded.HeightIncrement;
        }

        public void DecrementExpHeightAcc()
        {
            expHeightAcc -= Data.Stage.Loaded.HeightIncrement;
            expHeightAcc = expHeightAcc < 0f ? 0f : expHeightAcc;
        }
    }
}