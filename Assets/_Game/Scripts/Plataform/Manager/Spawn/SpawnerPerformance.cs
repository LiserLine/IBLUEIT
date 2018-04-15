using Ibit.Plataform.Data;
using NaughtyAttributes;
using System;
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

        public Action<float, float> OnUpdatedPerformanceTarget;
        public Action<float, float> OnUpdatedPerformanceObstacle;

        private void PerformanceOnPlayerHit(GameObject hit)
        {
            switch (hit.tag)
            {
                case "AirTarget":
                    airTargetsHit++;
                    TargetsSucceeded++;
                    if (airTargetsHit >= StageInfo.Loaded.HeightUpThreshold)
                    {
                        IncrementInsHeight();
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit++;
                    TargetsSucceeded++;
                    if (waterTargetsHit >= StageInfo.Loaded.HeightUpThreshold)
                    {
                        IncrementExpHeight();
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit--;
                    ObstaclesFailed++;
                    if (airObstaclesHit <= -StageInfo.Loaded.SizeDownThreshold)
                    {
                        DecrementExpSize();
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit--;
                    ObstaclesFailed++;
                    if (waterObstaclesHit <= -StageInfo.Loaded.SizeDownThreshold)
                    {
                        DecrementInsSize();
                        waterObstaclesHit = 0;
                    }
                    break;
            }
        }

        public void PerformanceOnPlayerMiss(string objectTag)
        {
            switch (objectTag)
            {
                case "AirTarget":
                    airTargetsHit--;
                    TargetsFailed++;
                    if (airTargetsHit <= -StageInfo.Loaded.HeightDownThreshold)
                    {
                        DecrementInsHeight();
                        airTargetsHit = 0;
                    }
                    break;

                case "WaterTarget":
                    waterTargetsHit--;
                    TargetsFailed++;
                    if (waterTargetsHit <= -StageInfo.Loaded.HeightDownThreshold)
                    {
                        DecrementExpHeight();
                        waterTargetsHit = 0;
                    }
                    break;

                case "AirObstacle":
                    airObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (airObstaclesHit >= StageInfo.Loaded.SizeUpThreshold)
                    {
                        IncrementExpSize();
                        airObstaclesHit = 0;
                    }
                    break;

                case "WaterObstacle":
                    waterObstaclesHit++;
                    ObstaclesSucceeded++;
                    if (waterObstaclesHit >= StageInfo.Loaded.SizeUpThreshold)
                    {
                        IncrementInsSize();
                        waterObstaclesHit = 0;
                    }
                    break;
            }
        }

        public void IncrementInsHeight()
        {
            insHeightAcc += StageInfo.Loaded.HeightIncrement;
            OnUpdatedPerformanceTarget?.Invoke(insHeightAcc, expHeightAcc);
        }

        public void DecrementInsHeight()
        {
            insHeightAcc -= StageInfo.Loaded.HeightIncrement;
            insHeightAcc = insHeightAcc < 0f ? 0f : insHeightAcc;
            OnUpdatedPerformanceTarget?.Invoke(insHeightAcc, expHeightAcc);
        }

        public void IncrementExpHeight()
        {
            expHeightAcc += StageInfo.Loaded.HeightIncrement;
            OnUpdatedPerformanceTarget?.Invoke(insHeightAcc, expHeightAcc);
        }

        public void DecrementExpHeight()
        {
            expHeightAcc -= StageInfo.Loaded.HeightIncrement;
            expHeightAcc = expHeightAcc < 0f ? 0f : expHeightAcc;
            OnUpdatedPerformanceTarget?.Invoke(insHeightAcc, expHeightAcc);
        }

        public void IncrementInsSize()
        {
            insSizeAcc += StageInfo.Loaded.SizeIncrement;
            OnUpdatedPerformanceObstacle?.Invoke(insSizeAcc, expSizeAcc);
        }

        public void DecrementInsSize()
        {
            insSizeAcc -= StageInfo.Loaded.SizeIncrement;
            insSizeAcc = insSizeAcc < 0f ? 0f : insSizeAcc;
            OnUpdatedPerformanceObstacle?.Invoke(insSizeAcc, expSizeAcc);
        }

        public void IncrementExpSize()
        {
            expSizeAcc += StageInfo.Loaded.SizeIncrement;
            OnUpdatedPerformanceObstacle?.Invoke(insSizeAcc, expSizeAcc);
        }

        public void DecrementExpSize()
        {
            expSizeAcc -= StageInfo.Loaded.SizeIncrement;
            expSizeAcc = expSizeAcc < 0f ? 0f : expSizeAcc;
            OnUpdatedPerformanceObstacle?.Invoke(insSizeAcc, expSizeAcc);
        }
    }
}