using Ibit.Core.Data;
using Ibit.Plataform.Camera;
using NaughtyAttributes;
using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner
    {
        [BoxGroup("Targets")] [SerializeField] private GameObject[] targetsAir;
        [BoxGroup("Targets")] [SerializeField] private GameObject[] targetsWater;

        private void DistanciateTargets(ref GameObject first, ref GameObject second)
        {
            second.transform.Translate(first.transform.position.x + Mathf.Clamp(Pacient.Loaded.Capacities.RespCycleDuration / 2500f, 1f, 2f) - second.transform.position.x, 0f, 0f);
        }

        private void InstantiateTargetAir(out GameObject spawned)
        {
            var index = Random.Range(0, targetsAir.Length);

            spawned = Instantiate(targetsAir[index],
                transform.position,
                transform.rotation,
                transform);

            //if (Data.Stage.Loaded.Level == 1)
            //{
            //    SpawnTutorialArrowAir(ref spawned);
            //}

            var posY = (1f + insHeightAcc) * CameraLimits.Boundary * Random.Range(0.2f, Data.Stage.Loaded.GameDifficulty);

            posY = Mathf.Clamp(posY, 0.2f * CameraLimits.Boundary, CameraLimits.Boundary);

            spawned.transform.Translate(0f, posY, 0f);
        }

        private void InstantiateTargetWater(out GameObject spawned)
        {
            var index = Random.Range(0, targetsWater.Length);

            spawned = Instantiate(targetsWater[index],
                transform.position,
                transform.rotation,
                transform);

            //if (Data.Stage.Loaded.Level == 1)
            //{
            //    SpawnTutorialArrowWater(ref spawned);
            //}

            var posY = (1f + expHeightAcc) * CameraLimits.Boundary * Random.Range(0.2f, Data.Stage.Loaded.GameDifficulty);

            posY = Mathf.Clamp(-posY, -CameraLimits.Boundary, 0.2f * -CameraLimits.Boundary);

            spawned.transform.Translate(0f, posY, 0f);
        }

        [Button("Release Targets")]
        private void ReleaseTargets()
        {
            GameObject airObj, waterObj;

            InstantiateTargetAir(out airObj);
            InstantiateTargetWater(out waterObj);

            DistanciateSpawns(ref airObj);
            DistanciateTargets(ref airObj, ref waterObj);

            UpdateSpeed(ref airObj);
            UpdateSpeed(ref waterObj);

            OnObjectReleased?.Invoke(ObjectToSpawn.Targets, ref airObj, ref waterObj);
        }
    }
}