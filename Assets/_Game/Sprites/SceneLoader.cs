﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private LoadingScreenUI loadingScreen;

    private void OnEnable()
    {
        if (loadingScreen == null)
            loadingScreen = FindObjectOfType<LoadingScreenUI>();
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadingScreen.Show();

        var operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingScreen.Slider.value = progress;

            Debug.Log($"LoadingScene - sceneIndex:{sceneIndex} progress:{progress}");

            yield return null;
        }
    }

    public void LoadScene(int sceneIndex) => StartCoroutine(LoadSceneAsync(sceneIndex));
}
