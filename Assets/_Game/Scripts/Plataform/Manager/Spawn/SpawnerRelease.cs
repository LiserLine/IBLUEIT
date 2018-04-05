using Ibit.Core.Data;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner
    {
        private float minDistanceBetweenSpawns = 2.5f;

        public delegate void ObjectReleasedHandler(ObjectToSpawn type, ref GameObject obj1, ref GameObject obj2);
        public event ObjectReleasedHandler OnObjectReleased;

        private bool spawnRelaxTime;

        public bool RelaxTimeSpawned { get; private set; }

        private void DistanciateSpawns(ref GameObject next)
        {
            var dist = minDistanceBetweenSpawns + (1f + (1f / (float)Pacient.Loaded.Condition));

            var lastObj = SpawnedObjects.Length > 2 ? SpawnedObjects[SpawnedObjects.Length - 3] : this.transform;
            var lastPos = lastObj.position.x + lastObj.localScale.x / 2f;

            var relativeDistance = next.transform.position.x - lastPos;

            if (relativeDistance > 0 && relativeDistance < dist)
                next.transform.Translate(dist - relativeDistance, 0f, 0f);
            else if (relativeDistance < dist && relativeDistance < 0)
                next.transform.Translate(-relativeDistance + dist, 0f, 0f);
        }

        [Button("Release")]
        private void Spawn()
        {
            switch (Data.Stage.Loaded.ObjectToSpawn)
            {
                case ObjectToSpawn.Targets:
                    ReleaseTargets();
                    break;

                case ObjectToSpawn.Obstacles:
                    ReleaseObstacles();
                    break;

                default:
                    if (Random.Range(0, 2) == 0)
                        ReleaseTargets();
                    else
                        ReleaseObstacles();
                    break;
            }
        }

        private void UpdateSpeed(ref GameObject go) => go.GetComponent<MoveObject>().Speed = Data.Stage.Loaded.ObjectSpeedMultiplier;
    }
}