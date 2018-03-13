﻿using Ibit.Core.Data;
using Ibit.Plataform.Camera;
using UnityEngine;

namespace Ibit.Plataform.Manager.Spawn
{
    public partial class Spawner
    {
        private void ReleaseRelaxTime()
        {
            if (!spawnRelaxTime || RelaxTimeSpawned)
                return;

            var disfunction = (int)Pacient.Loaded.Condition;
            var objects = new GameObject[11 + 4 * disfunction];
            int i;

            Vector3 refPos;
            if (SpawnedObjects.Length > 2)
            {
                refPos = SpawnedObjects[SpawnedObjects.Length - 3].position;
                minDistanceBetweenSpawns += 3f;
            }
            else
                refPos = this.transform.position;

            refPos.y = 0;

            for (i = 0; i < 4; i++)
            {
                objects[i] = Instantiate(relaxInsPrefab, refPos, transform.rotation, transform);

                if (i != 0)
                    continue;

                DistanciateSpawns(ref objects[0]);
                refPos = objects[0].transform.position;
                refPos.y = 0;
            }

            for (; i < 11; i++)
                objects[i] = Instantiate(relaxZeroPrefab, refPos, transform.rotation, transform);

            for (; i < objects.Length; i++)
                objects[i] = Instantiate(relaxExpPrefab, refPos, transform.rotation, transform);

            for (i = 0; i < objects.Length; i++)
            {
                UpdateSpeed(ref objects[i]);
                objects[i].transform.Translate(i / 1.5f, 0f, 0f);
            }

            for (i = 0; i < objects.Length; i++)
            {
                if (i < 4)
                    objects[i].transform.Translate(0f, 0.2f * CameraLimits.Boundary, 0f);
                else if (i > 10)
                    objects[i].transform.Translate(0f, 0.15f * -CameraLimits.Boundary, 0f);
            }

            RelaxTimeSpawned = true;
        }
    }
}