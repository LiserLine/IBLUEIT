using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (!LocalizationManager.Instance.IsReady
                || !GameDataManager.Instance.IsReady)
        {
            yield return null;
        }

        Debug.Log("System initialized successfully!");

        SceneManager.LoadScene(4);
    }
}
