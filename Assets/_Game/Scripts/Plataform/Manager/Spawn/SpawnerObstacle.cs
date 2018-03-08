using Ibit.Core.Data;
using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner
    {
        private void DistanciateObstacles(ref GameObject first, ref GameObject second)
        {
            var firstPos = first.transform.position.x + first.transform.localScale.x / 2f;
            var secondPos = second.transform.position.x - second.transform.localScale.x / 2f;

            var limit = Pacient.Loaded.Capacities.RespCycleDuration / 3000f;

            second.transform.Translate(firstPos + Mathf.Clamp(limit, 2.5f, 3.5f) - secondPos, 0f, 0f);
        }

        private void InstanciateObstacleAir(out GameObject spawned)
        {
            var index = Random.Range(0, obstaclesAir.Length);

            spawned = Instantiate(obstaclesAir[index],
                new Vector3(transform.position.x, 0f),
                transform.rotation,
                transform);

            var scale = Pacient.Loaded.Capacities.ExpFlowDuration / 1000f * (1f + expSizeAcc) * Data.Stage.Loaded.GameDifficulty;

            scale = scale < 1f ? 1f : scale;

            spawned.transform.localScale = new Vector3(scale, scale, 1);
            spawned.transform.Translate(0f, spawned.transform.localScale.y / 2, 0f);
        }

        private void InstanciateObstacleWater(out GameObject spawned)
        {
            var index = Random.Range(0, obstaclesWater.Length);

            spawned = Instantiate(obstaclesWater[index],
                new Vector3(transform.position.x, 0f),
                transform.rotation,
                transform);

            var scale = Pacient.Loaded.Capacities.InsFlowDuration / 1000f * (1f + insSizeAcc) * Data.Stage.Loaded.GameDifficulty;

            scale = scale < 1f ? 1f : scale;

            spawned.transform.localScale = new Vector3(scale, scale, 1);
            spawned.transform.Translate(0f, -spawned.transform.localScale.y / 2, 0f);
        }

        private void ReleaseObstacles()
        {
            GameObject airObj, waterObj;

            InstanciateObstacleWater(out waterObj);
            InstanciateObstacleAir(out airObj);

            DistanciateSpawns(ref waterObj);
            DistanciateObstacles(ref waterObj, ref airObj);

            UpdateSpeed(ref airObj);
            UpdateSpeed(ref waterObj);

            OnObjectReleased?.Invoke(ObjectToSpawn.Obstacles, ref waterObj, ref airObj);
        }
    }
}