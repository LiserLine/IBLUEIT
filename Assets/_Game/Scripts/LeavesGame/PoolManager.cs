using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ibit.LeavesGame
{
    public class PoolManager : MonoBehaviour
    {

        public List<GameObject> objPool = new List<GameObject>();
        private int objPoolSize;

        public static PoolManager _instance;

        public static PoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PoolManager>();
                }
                return _instance;
            }
        }

        public void CreatePool(GameObject objPrefab, int poolSize)
        {
            objPoolSize = poolSize;
            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(objPrefab) as GameObject;
                newObject.SetActive(false);
                objPool.Add(newObject);
            }
        }

        public GameObject GetObject()
        {
            if (objPool.Count > 0)
            {
                GameObject obj = objPool[0];
                objPool.RemoveAt(0);
                return obj;
            }
            return null;
        }

        public void DestroyObjectPool(GameObject obj)
        {
            objPool.Add(obj);
            obj.SetActive(false);
        }
        public void ClearPool()
        {
            objPool.Clear();
            objPool = null;
        }

    }


}
