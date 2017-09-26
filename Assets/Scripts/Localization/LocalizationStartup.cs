using System.Collections;
using System.IO;
using UnityEngine;

public class LocalizationStartup : MonoBehaviour
{
    public GameObject SelectLanguageScreen;
    public LevelLoader LevelLoader;

    private IEnumerator Start()
    {
        if (!File.Exists(GameConstants.LocalizationPath))
        {
            SelectLanguageScreen.SetActive(true);
        }
        else
        {
            var localization = GameUtilities.ReadAllText(GameConstants.LocalizationPath);
            LocalizationManager.Instance.LoadLocalizationData(localization);
        }

        while (!LocalizationManager.Instance.IsReady)
        {
            yield return null;
        }

        LevelLoader.LoadScene(1);

        Destroy(this);
    }
}
