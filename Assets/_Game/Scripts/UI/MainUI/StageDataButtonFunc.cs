using UnityEngine;

public class StageDataButtonFunc : MonoBehaviour
{
    public void LoadStage()
    {
        Spawner.StageToLoad = this.GetComponent<StageHolder>().StageToLoad;
        SceneLoader.Instance.LoadScene(1);
    }
}

public class StageHolder : MonoBehaviour
{ public int StageToLoad { get; set; } }
