using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ibit.LeavesGame
{
    public class Spawner : MonoBehaviour
    {
        public GameObject prefab;
        public bool passed = false;
        public GameObject lastObj;
        public int poolSize;

        void Awake()
        {
            PoolManager.Instance.CreatePool(prefab, poolSize);
        }

        void Start()
        {
            InitializeObjects();
        }

        private void InitializeObjects()
        {
            float last_xPos = 0;
            Debug.Log(PoolManager.Instance.objPool.Count);
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = PoolManager.Instance.GetObject();
                if (!passed)
                {
                    obj.transform.position = new Vector3(last_xPos, 2, -0.1f);
                    last_xPos += 2;
                    passed = true;
                }
                else
                {
                    obj.transform.position = new Vector3(last_xPos, -2, -0.1f);
                    last_xPos += 2;
                    passed = false;
                }
                obj.SetActive(true);
                lastObj = obj;
            }
        }

        public void SpawnObject()
        {
            Vector3 lastPos = lastObj.transform.position;
            lastPos.x += 2;
            lastPos.y *= -1;

            GameObject obj = PoolManager.Instance.GetObject();
            obj.transform.position = lastPos;
            obj.SetActive(true);
            lastObj = obj;
        }
    }
}
