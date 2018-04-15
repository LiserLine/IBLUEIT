﻿using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Score;
using NaughtyAttributes;
using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner
    {
        [BoxGroup("Targets")] [SerializeField] private GameObject[] targetsAir;
        [BoxGroup("Targets")] [SerializeField] private GameObject[] targetsWater;

        private void SpawnTarget(StageObject stageObject)
        {
            GameObject instance;

            if (stageObject.PositionYFactor > 0) //air
            {
                instance = Instantiate(targetsAir[Random.Range(0, targetsAir.Length)],
                    new Vector3(_lastSpawned.position.x + stageObject.PositionXSpacing, 0f),
                    this.transform.rotation,
                    this.transform);
            }
            else //water
            {
                instance = Instantiate(targetsWater[Random.Range(0, targetsWater.Length)],
                    new Vector3(_lastSpawned.position.x + stageObject.PositionXSpacing, 0f),
                    this.transform.rotation,
                    this.transform);
            }

            instance.AddComponent<Target>().Build(stageObject);

            UpdateSpeed(ref instance);

            FindObjectOfType<Scorer>().UpdateMaxScore(stageObject.Type, ref instance, stageObject.DifficultyFactor);
        }
    }
}