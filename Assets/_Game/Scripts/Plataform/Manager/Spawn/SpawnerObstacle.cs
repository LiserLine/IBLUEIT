using Ibit.Core.Data;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Score;
using NaughtyAttributes;
using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner
    {
        [BoxGroup("Obstacles")] [SerializeField] private GameObject[] obstaclesAir;
        [BoxGroup("Obstacles")] [SerializeField] private GameObject[] obstaclesWater;

        private void SpawnObstacle(StageObject stageObject)
        {
            GameObject instance;

            //air
            if (stageObject.PositionYFactor > 0)
            {
                instance = Instantiate(obstaclesAir[Random.Range(0, obstaclesAir.Length)],
                new Vector3(_lastSpawned.position.x + stageObject.PositionXSpacing, 0f),
                this.transform.rotation,
                this.transform);

                if (StageInfo.Loaded.Level == 1)
                {
                    SpawnTutorialArrowWater(ref instance);
                }
            }

            //underwater
            else
            {
                instance = Instantiate(obstaclesWater[Random.Range(0, obstaclesWater.Length)],
                new Vector3(_lastSpawned.position.x + stageObject.PositionXSpacing, 0f),
                this.transform.rotation,
                this.transform);

                if (StageInfo.Loaded.Level == 1)
                {
                    SpawnTutorialArrowAir(ref instance);
                }
            }

            instance.AddComponent<Obstacle>().Build(stageObject);

            UpdateSpeed(ref instance);

            FindObjectOfType<Scorer>().UpdateMaxScore(stageObject.Type, ref instance, stageObject.DifficultyFactor);
        }
    }
}