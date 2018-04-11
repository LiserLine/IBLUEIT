using Ibit.Plataform.Camera;
using Ibit.Plataform.Data;
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
                InstantiateTargetAir(out instance, stageObject.DifficultyFactor);
            else //water
                InstantiateTargetWater(out instance, stageObject.DifficultyFactor);

            var target = instance.AddComponent<Target>();
            target.Properties = stageObject;

            UpdateSpeed(ref instance);
            DistantiateFromLastSpawned(ref instance, stageObject.PositionXSpacing);

            FindObjectOfType<Scorer>().UpdateMaxScore(stageObject.Type, ref instance, stageObject.DifficultyFactor);
        }

        private void InstantiateTargetAir(out GameObject o, float difficultyFactor)
        {
            var index = Random.Range(0, targetsAir.Length);

            o = Instantiate(targetsAir[index],
                this.transform.position,
                this.transform.rotation,
                this.transform);

            var posY = (1f + insHeightAcc) * CameraLimits.Boundary * Random.Range(0.2f, difficultyFactor);

            posY = Mathf.Clamp(posY, 0.2f * CameraLimits.Boundary, CameraLimits.Boundary);

            o.transform.Translate(0f, posY, 0f);
        }

        private void InstantiateTargetWater(out GameObject o, float difficultyFactor)
        {
            var index = Random.Range(0, targetsWater.Length);

            o = Instantiate(targetsWater[index],
                this.transform.position,
                this.transform.rotation,
                this.transform);

            var posY = (1f + expHeightAcc) * CameraLimits.Boundary * Random.Range(0.2f, difficultyFactor);

            posY = Mathf.Clamp(-posY, -CameraLimits.Boundary, 0.2f * -CameraLimits.Boundary);

            o.transform.Translate(0f, posY, 0f);
        }
    }
}