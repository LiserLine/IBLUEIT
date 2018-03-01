using Ibit.Core.Audio;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Spawn;
using System.Linq;
using UnityEngine;

namespace Ibit.Plataform
{
    public class BGMPlayer : MonoBehaviour
    {
        private int numDay, numAfternoon, numNight;

        private void Awake()
        {
            numDay = SoundManager.Instance.Sounds.Count(x => x.name.Contains("BGM_Day"));
            numAfternoon = SoundManager.Instance.Sounds.Count(x => x.name.Contains("BGM_Afternoon"));
            numNight = SoundManager.Instance.Sounds.Count(x => x.name.Contains("BGM_Night"));
        }

        private void PlayBGM()
        {
            switch (Stage.Loaded.ObjectToSpawn)
            {
                case ObjectToSpawn.Targets:
                    SoundManager.Instance.PlaySound($"BGM_Day{Random.Range(1, numDay)}");
                    break;

                case ObjectToSpawn.TargetsAndObstacles:
                    SoundManager.Instance.PlaySound($"BGM_Afternoon{Random.Range(1, numAfternoon)}");
                    break;

                case ObjectToSpawn.Obstacles:
                    SoundManager.Instance.PlaySound($"BGM_Night{Random.Range(1, numNight)}");
                    break;
            }
        }

        private void Start() => PlayBGM();
    }
}