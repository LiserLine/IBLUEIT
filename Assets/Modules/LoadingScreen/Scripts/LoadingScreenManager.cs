using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    // REMEMBER TO CHANGE THIS VARIABLE ON EVERY NEW PROJECT
    private static int _loadingScreenSceneIndex = 1;
    private static int _sceneIndex = -1;

    public Slider Slider;

    public static void LoadScene(int sceneToLoad)
    {
        _sceneIndex = sceneToLoad;
        SceneManager.LoadScene(_loadingScreenSceneIndex);
    }

    private void Start()
    {
        if (_sceneIndex < 0)
            return;

        StartCoroutine(LoadTargetSceneAsync());
    }

    private IEnumerator LoadTargetSceneAsync()
    {
        var operation = SceneManager.LoadSceneAsync(_sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            Debug.LogFormat("LoadingScene - sceneIndex:{0} progress:{1}",
                _sceneIndex, progress);

            Slider.value = progress;

            yield return null;
        }
    }
}
