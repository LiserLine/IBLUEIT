using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Stage;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner : MonoBehaviour
    {
        [BoxGroup("Tutorial")] [SerializeField] private GameObject arrowUpPrefab;
        [BoxGroup("Tutorial")] [SerializeField] private GameObject arrowDownPrefab;

        public Transform[] SpawnedObjects
        {
            get
            {
                return this.GetComponentsInChildren<Transform>().
                    Where(o => !o.name.Equals("Spawner") && !o.name.Equals("Sprite")).ToArray();
            }
        }

        public int ObjectsOnScene => SpawnedObjects.Length;

        private Transform _lastSpawned => SpawnedObjects.Length > 0 ? SpawnedObjects[SpawnedObjects.Length - 1] : this.transform;

        private void Awake()
        {
            var stgMgr = FindObjectOfType<StageManager>();
            stgMgr.OnStageStart += Spawn;
            stgMgr.OnStageEnd += Clean;

            FindObjectOfType<Player>().OnObjectHit += PerformanceOnHit;
        }

        [Button("Spawn")]
        private void Spawn()
        {
            for (int i = 0; i < StageInfo.Loaded.Loops; i++)
            {
                foreach (var stageObject in StageInfo.Loaded.StageObjects)
                {
                    switch (stageObject.Type)
                    {
                        case StageObjectType.Target:
                            SpawnTarget(stageObject);
                            break;
                        case StageObjectType.Obstacle:
                            SpawnObstacle(stageObject);
                            break;
                        case StageObjectType.Relax:
                            SpawnRelax(stageObject.PositionXSpacing);
                            break;
                    }
                }
            }
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

        private void UpdateSpeed(ref GameObject obj)
        {
            obj.GetComponent<MoveObject>().Speed = StageInfo.Loaded.ObjectSpeedFactor;
        }

        private void SpawnTutorialArrowAir(ref GameObject obj)
        {
            var arrow = Instantiate(arrowUpPrefab, obj.transform);
            arrow.transform.Translate(0f, obj.transform.localScale.y / 2f + 1f, 0f);
        }

        private void SpawnTutorialArrowWater(ref GameObject obj)
        {
            var arrow = Instantiate(arrowDownPrefab, obj.transform);
            arrow.transform.Translate(0f, obj.transform.localScale.y / 2f - 2f, 0f);
        }
    }
}