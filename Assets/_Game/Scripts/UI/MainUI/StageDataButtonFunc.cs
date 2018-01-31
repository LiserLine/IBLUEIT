using UnityEngine;

public class StageDataButtonFunc : MonoBehaviour
{
    public void LoadStage()
    {
        if (!SerialController.Instance.IsConnected)
        {
            SysMessage.Warning("PITACO não está conectado! Verifique a conexão!");
            return;
        }

        Spawner.StageToLoad = this.GetComponent<StageHolder>().StageToLoad;
        SceneLoader.Instance.LoadScene(1);
    }
}

public class StageHolder : MonoBehaviour
{ public int StageToLoad { get; set; } }
