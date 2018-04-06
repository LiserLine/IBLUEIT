using Ibit.Core.Data;
using Ibit.Plataform.Manager.Stage;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public enum ObjectToSpawn
    {
        Targets = 1,
        TargetsAndObstacles = 2,
        Obstacles = 3
    }

    public partial class Spawner : MonoBehaviour
    {
        [BoxGroup("Tutorial")] [SerializeField] private GameObject arrowUpPrefab;
        [BoxGroup("Tutorial")] [SerializeField] private GameObject arrowDownPrefab;

        private bool spawnEnabled;
        private float timer;

        private float minDistanceBetweenSpawns = 2.5f;

        public int ObjectsOnScene => SpawnedObjects.Length;
        public Transform[] SpawnedObjects => this.gameObject.GetComponentsInChildren<Transform>().Where(o => !o.name.Equals("Spawner") && !o.name.Equals("Sprite")).ToArray();

        public delegate void ObjectReleasedHandler(ObjectToSpawn type, ref GameObject obj1, ref GameObject obj2);
        public event ObjectReleasedHandler OnObjectReleased;

        //public Action<ObjectToSpawn, GameObject, GameObject> OnObjectReleased;

        private void Awake()
        {
            var stgMgr = FindObjectOfType<StageManager>();
            stgMgr.OnStageStart += EnableSpawn;
            stgMgr.OnStageTimeUp += ReleaseRelaxTime;
            stgMgr.OnStageTimeUp += DisableSpawn;
            stgMgr.OnStageEnd += Clean;

            if (Data.Stage.Loaded.ObjectToSpawn == ObjectToSpawn.Obstacles)
                spawnRelaxTime = true;

            var plr = FindObjectOfType<Player>();
            plr.OnPlayerDeath += DisableSpawn;
            plr.OnEnemyHit += Player_OnEnemyHit;

            timer = Data.Stage.Loaded.SpawnDelay;
        }

        private void Update()
        {
            if (!spawnEnabled)
                return;

            timer += Time.deltaTime;

            if (timer > Data.Stage.Loaded.SpawnDelay)
            {
                timer = 0f;
                Spawn();
            }
        }

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

        [Button("Enable Spawn")]
        private void EnableSpawn()
        {
            if (spawnEnabled)
                return;

            spawnEnabled = true;
            timer = Data.Stage.Loaded.SpawnDelay;
        }

        [Button("Disable Spawn")]
        private void DisableSpawn()
        {
            if (!spawnEnabled)
                return;

            spawnEnabled = false;
            timer = 0f;
        }

        private void Clean()
        {
            if (SpawnedObjects.Length < 1)
                return;

            foreach (var tform in SpawnedObjects)
            {
                if (tform == this.gameObject.transform)
                    continue;

                Destroy(tform.gameObject);
            }
        }

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

        private void UpdateSpeed(ref GameObject go)
        {
            go.GetComponent<MoveObject>().Speed = Data.Stage.Loaded.ObjectSpeedMultiplier;
        }

        private void SpawnTutorialArrowAir(ref GameObject objRef)
        {
            var arrow = Instantiate(arrowUpPrefab, objRef.transform);
            arrow.transform.Translate(0f, objRef.transform.localScale.y / 2f + 1f, 0f);
        }

        private void SpawnTutorialArrowWater(ref GameObject objRef)
        {
            var arrow = Instantiate(arrowDownPrefab, objRef.transform);
            arrow.transform.Translate(0f, objRef.transform.localScale.y / 2f - 2f, 0f);
        }
    }
}