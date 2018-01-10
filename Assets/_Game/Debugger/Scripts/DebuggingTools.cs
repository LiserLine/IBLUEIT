using UnityEngine;
using UnityEngine.SceneManagement;

public class DebuggingTools : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;

    public void Awake()
    {
#if UNITY_EDITOR
#else
        Destroy(this.gameObject);
#endif
    }

    [ContextMenu("Load Selected Scene")]
    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
