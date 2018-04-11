using Ibit.Plataform.Data;
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

        private void PerformanceOnHit(GameObject hit)
        {
            switch (hit.tag)
            {
                case "AirTarget":
                    airTargetsHit++;
                    TargetsSucceeded++;
                    if (airTargetsHit >= StageInfo.Loaded.HeightUpThreshold)
                    {
                        IncrementInsHeightAcc();
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit++;
                    TargetsSucceeded++;
                    if (waterTargetsHit >= StageInfo.Loaded.HeightUpThreshold)
                    {
                        IncrementExpHeightAcc();
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit--;
                    ObstaclesFailed++;
                    if (airObstaclesHit <= -StageInfo.Loaded.SizeDownThreshold)
                    {
                        DecrementExpSizeAcc();
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit--;
                    ObstaclesFailed++;
                    if (waterObstaclesHit <= -StageInfo.Loaded.SizeDownThreshold)
                    {
                        DecrementInsSizeAcc();
                        waterObstaclesHit = 0;
                    }
                    break;
            }
        }

        public void PerformanceOnMiss(string objectTag)
        {
            switch (objectTag)
            {
                case "AirTarget":
                    airTargetsHit--;
                    TargetsFailed++;
                    if (airTargetsHit <= -StageInfo.Loaded.HeightDownThreshold)
                    {
                        DecrementInsHeightAcc();
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit--;
                    TargetsFailed++;
                    if (waterTargetsHit <= -StageInfo.Loaded.HeightDownThreshold)
                    {
                        DecrementExpHeightAcc();
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (airObstaclesHit >= StageInfo.Loaded.SizeUpThreshold)
                    {
                        IncrementExpSizeAcc();
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (waterObstaclesHit >= StageInfo.Loaded.SizeUpThreshold)
                    {
                        IncrementInsSizeAcc();
                        waterObstaclesHit = 0;
                    }
                    break;
            }
        }

        public void IncrementInsSizeAcc()
        {
            insSizeAcc += StageInfo.Loaded.SizeIncrement;
        }

        public void DecrementInsSizeAcc()
        {
            insSizeAcc -= StageInfo.Loaded.SizeIncrement;
            insSizeAcc = insSizeAcc < 0f ? 0f : insSizeAcc;
        }

        public void IncrementInsHeightAcc()
        {
            insHeightAcc += StageInfo.Loaded.HeightIncrement;
        }

        public void DecrementInsHeightAcc()
        {
            insHeightAcc -= StageInfo.Loaded.HeightIncrement;
            insHeightAcc = insHeightAcc < 0f ? 0f : insHeightAcc;
        }

        public void IncrementExpSizeAcc()
        {
            expSizeAcc += StageInfo.Loaded.SizeIncrement;
        }

        public void DecrementExpSizeAcc()
        {
            expSizeAcc -= StageInfo.Loaded.SizeIncrement;
            expSizeAcc = expSizeAcc < 0f ? 0f : expSizeAcc;
        }

        public void IncrementExpHeightAcc()
        {
            expHeightAcc += StageInfo.Loaded.HeightIncrement;
        }

        public void DecrementExpHeightAcc()
        {
            expHeightAcc -= StageInfo.Loaded.HeightIncrement;
            expHeightAcc = expHeightAcc < 0f ? 0f : expHeightAcc;
        }
    }
}