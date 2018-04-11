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
                InstantiateTargetAir(out instance, stageObject.DifficultyFactor, stageObject.PositionXSpacing);
            else //water
                InstantiateTargetWater(out instance, stageObject.DifficultyFactor, stageObject.PositionXSpacing);

            var target = instance.AddComponent<Target>();
            target.Properties = stageObject;

            UpdateSpeed(ref instance);

            FindObjectOfType<Scorer>().UpdateMaxScore(stageObject.Type, ref instance, stageObject.DifficultyFactor);
        }

        private void InstantiateTargetAir(out GameObject o, float difficultyFactor, float spacing)
        {
            var index = Random.Range(0, targetsAir.Length);

            o = Instantiate(targetsAir[index],
                new Vector3(_lastSpawned.position.x + spacing, 0f),
                this.transform.rotation,
                this.transform);

            var posY = (1f + insHeightAcc) * CameraLimits.Boundary * difficultyFactor;

            posY = Mathf.Clamp(posY, 0.2f * CameraLimits.Boundary, CameraLimits.Boundary);

            o.transform.Translate(0f, posY, 0f);
        }

        private void InstantiateTargetWater(out GameObject o, float difficultyFactor, float spacing)
        {
            var index = Random.Range(0, targetsWater.Length);

            o = Instantiate(targetsWater[index],
                new Vector3(_lastSpawned.position.x + spacing, 0f),
                this.transform.rotation,
                this.transform);

            var posY = (1f + expHeightAcc) * CameraLimits.Boundary * difficultyFactor;

            posY = Mathf.Clamp(-posY, -CameraLimits.Boundary, 0.2f * -CameraLimits.Boundary);

            o.transform.Translate(0f, posY, 0f);
        }
    }
}