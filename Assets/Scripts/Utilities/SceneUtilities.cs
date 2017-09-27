using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtilities : MonoBehaviour
{
    private void Update()
    {
        EscapeHotkey();
    }

    private void EscapeHotkey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
    }
}
