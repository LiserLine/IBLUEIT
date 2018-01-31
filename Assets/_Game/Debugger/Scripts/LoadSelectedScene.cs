using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Debugger
{
    [SerializeField]
    private string sceneToLoad;

    [ContextMenu("Load Selected Scene")]
    private void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
