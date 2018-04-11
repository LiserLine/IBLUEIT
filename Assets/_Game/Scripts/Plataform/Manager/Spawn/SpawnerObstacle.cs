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

            if (stageObject.PositionYFactor > 0) //air
                InstantiateObstacleAir(out instance, stageObject.DifficultyFactor, stageObject.PositionXSpacing);
            else //water
                InstantiateObstacleWater(out instance, stageObject.DifficultyFactor, stageObject.PositionXSpacing);

            var obstacle = instance.AddComponent<Obstacle>();
            obstacle.Properties = stageObject;

            UpdateSpeed(ref instance);

            FindObjectOfType<Scorer>().UpdateMaxScore(stageObject.Type, ref instance, stageObject.DifficultyFactor);
        }

        private void InstantiateObstacleAir(out GameObject o, float difficultyFactor, float spacing)
        {
            var index = Random.Range(0, obstaclesAir.Length);

            o = Instantiate(obstaclesAir[index],
                new Vector3(_lastSpawned.position.x + spacing, 0f),
                this.transform.rotation,
                this.transform);

            if (StageInfo.Loaded.Level == 1)
            {
                SpawnTutorialArrowWater(ref o);
            }

            var scale = Pacient.Loaded.Capacities.ExpFlowDuration / 1000f * (1f + expSizeAcc) * difficultyFactor;

            scale = scale < 1f ? 1f : scale;

            o.transform.localScale = new Vector3(scale, scale, 1f);
            o.transform.Translate(0f, o.transform.localScale.y / 2f, 0f);
        }

        private void InstantiateObstacleWater(out GameObject o, float difficultyFactor, float spacing)
        {
            var index = Random.Range(0, obstaclesWater.Length);

            o = Instantiate(obstaclesWater[index],
                new Vector3(_lastSpawned.position.x + spacing, 0f),
                this.transform.rotation,
                this.transform);

            if (StageInfo.Loaded.Level == 1)
            {
                SpawnTutorialArrowAir(ref o);
            }

            var scale = Pacient.Loaded.Capacities.InsFlowDuration / 1000f * (1f + insSizeAcc) * difficultyFactor;

            scale = scale < 1f ? 1f : scale;

            o.transform.localScale = new Vector3(scale, scale, 1f);
            o.transform.Translate(0f, -o.transform.localScale.y / 2f, 0f);
        }
    }
}