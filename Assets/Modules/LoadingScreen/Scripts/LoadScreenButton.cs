using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScreenButton : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex > SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogErrorFormat("Can't load scene number {0}. SceneManager only has {1} scenes in BuildSettings!",
                sceneIndex, SceneManager.sceneCountInBuildSettings);

            return;
        }

        LoadingScreenManager.LoadScene(sceneIndex);
    }
}
