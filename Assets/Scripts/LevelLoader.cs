using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider Slider;

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadingScreen.SetActive(true);

        var operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            Slider.value = progress;

            Debug.LogFormat("LoadingScene - sceneIndex:{0} progress:{1}",
                sceneIndex, progress);

            yield return null;
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
}
