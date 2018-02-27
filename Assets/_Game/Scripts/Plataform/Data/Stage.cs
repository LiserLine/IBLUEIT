using Ibit.Plataform.Manager.Spawn;
using System;
using UnityEngine;

namespace Ibit.Plataform.Data
{
    [Serializable]
    public class Stage
    {
        public static Stage Loaded;

        [Header("Info")]

        public int Id;
        public ObjectToSpawn ObjectToSpawn;
        public int Level;

        [Space(5)]
        [Header("Stage Settings")]

        public float SpawnDelay;
        public int SpawnDuration;
        public float GameDifficulty;
        public float ObjectSpeedMultiplier;

        [Space(5)]
        [Header("Target Settings")]

        public float HeightIncrement;
        public float HeightLevelUpThreshold;
        public float HeightLevelDownThreshold;

        [Space(5)]
        [Header("Obstacle Settings")]

        public float SizeIncrement;
        public float SizeLevelUpThreshold;
        public float SizeLevelDownThreshold;

        [Space(5)]
        [Header("Relax Settings")]

        public int RelaxTimeThreshold;
    }
}