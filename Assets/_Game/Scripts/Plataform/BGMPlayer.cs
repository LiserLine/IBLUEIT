using System.Linq;
using UnityEngine;

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
        switch (Stage.Loaded.SpawnObject)
        {
            case SpawnObject.Targets:
                SoundManager.Instance.PlaySound($"BGM_Day{Random.Range(1, numDay)}");
                break;

            case SpawnObject.TargetsAndObstacles:
                SoundManager.Instance.PlaySound($"BGM_Afternoon{Random.Range(1, numAfternoon)}");
                break;

            case SpawnObject.Obstacles:
                SoundManager.Instance.PlaySound($"BGM_Night{Random.Range(1, numNight)}");
                break;
        }
    }

    private void Start() => PlayBGM();
}