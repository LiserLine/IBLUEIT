using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Debugger
{
    [SerializeField]
    private string sceneToLoad;

    [ContextMenu("Load Selected Scene")]
    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}